using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scap.Api;
using Scap.Bridge;
using Scap.Logic;
using Scap.Probes;
using Scap.Bridge.Oval;
using Scap.Bridge.Oval.Schema;
using tview.scapproc.shellv1.Objects;
using static ICSharpCode.AvalonEdit.Document.TextDocumentWeakEventManager;
using System.Windows.Forms;
using tview.scapproc.shellv1.Enums;
using System.IO;
using Newtonsoft.Json;
using Scap.Bridge.Oval.Model.Executable;
using wshell.Objects;

namespace tview.scapproc.shellv1
{
    public class ScapProcessor
    {
        public string XsdPath { get; private set; }
        public string DefenitionsPath { get; private set; }

        public EventRedirect OutputRedirect { get; set; }

        public event EventHandler<ScapState> StateChanged;
        public event EventHandler<string> OvalInfoLoaded;

        private List<Defenition> defenitionsCritical = new List<Defenition>();
        private List<Defenition> defenitionsHigh = new List<Defenition>();
        private List<Defenition> defenitionsMedium = new List<Defenition>();
        private List<Defenition> defenitionsLow = new List<Defenition>();
        private List<Defenition> defenitionsInventory = new List<Defenition>();

        public Defenition[] CriticalDefenitions => defenitionsCritical.ToArray();
        public Defenition[] HighDefenitions => defenitionsHigh.ToArray();
        public Defenition[] MediumDefenitions => defenitionsMedium.ToArray();
        public Defenition[] LowDefenitions => defenitionsLow.ToArray();
        public Defenition[] InventoryDefenitions => defenitionsInventory.ToArray();

        private DefinitionEvaluator _defenitionEvaluator;

        public ScapProcessor(string xsdPath, string defenitionsPath) 
        {
            XsdPath = xsdPath;
            DefenitionsPath = defenitionsPath;
        }

        public async Task StartAsync(CancellationToken token)
        {
            StateChanged?.Invoke(this, ScapState.Initialize);
            await Task.Run(() =>
            {
                var ns = new Ns(XsdPath);
                _defenitionEvaluator = new DefinitionEvaluator(ns);
                _defenitionEvaluator.OvalStatusChanged += OnChanged;

                StateChanged?.Invoke(this, ScapState.LoadDefenitions);
                OvalInfoLoaded?.Invoke(this, $"Загрузка...");

                _defenitionEvaluator.Load(DefenitionsPath, null, ValidationKind.VerifyLinks);

                StateChanged?.Invoke(this, ScapState.Eval);
                OvalInfoLoaded?.Invoke(this, $"");

                var resultDictionary = _defenitionEvaluator.Evaluate();
                var modifyResult = resultDictionary.Where(x => x.Value == Scap.Bridge.Oval.Model.ResultEnumeration.True).ToArray();

                StateChanged?.Invoke(this, ScapState.GetInfo);
                OvalInfoLoaded?.Invoke(this, $"Получение информации");

                int loaded = 0;
                int of = modifyResult.Length;

                foreach (var item in _defenitionEvaluator.Document.Definitions)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    if (resultDictionary[item.Key] != Scap.Bridge.Oval.Model.ResultEnumeration.True)
                        continue;

                    OvalInfoLoaded?.Invoke(this, $"Получение информации {++loaded}/{of}");
                    var risk = item.Key.GetRiskLevel();

                    var cls = "";
                    switch (item.Value.Class)
                    {
                        case Scap.Bridge.Oval.Model.ClassEnumeration.Compliance:
                            cls = "Конфигурация";
                            break;
                        case Scap.Bridge.Oval.Model.ClassEnumeration.Inventory:
                            cls = "Инвентарь";
                            break;
                        case Scap.Bridge.Oval.Model.ClassEnumeration.Miscellaneous:
                            cls = "Прочее";
                            break;
                        case Scap.Bridge.Oval.Model.ClassEnumeration.Patch:
                            cls = "Обновление";
                            break;
                        case Scap.Bridge.Oval.Model.ClassEnumeration.Vulnerability:
                            cls = "Уязвимость";
                            break;
                    }
                    OvalInfoLoaded?.Invoke(this, $"Распределен в {cls} [{item.Key}]");

                    if (item.Value.Class == Scap.Bridge.Oval.Model.ClassEnumeration.Inventory)
                    {
                        defenitionsInventory.Add(new Defenition
                        {
                            Id = item.Key,
                            Title = item.Value.Metadata.Title.Replace(" is installed", ""),
                            Class = cls,
                            Risk = "Информация",
                            Description = item.Value.Metadata.Description
                        });
                    }
                    else
                    {
                        switch (risk)
                        {
                            case Enums.RiskLevel.Critical:
                                defenitionsCritical.Add(new Defenition
                                {
                                    Id = item.Key,
                                    Title = item.Value.Metadata.Title,
                                    Class = cls,
                                    Risk = "Критический",
                                    Description = item.Value.Metadata.Description,
                                    Hyperlink = GetHyperlink(item.Value)
                                });
                                break;
                            case Enums.RiskLevel.High:
                                defenitionsHigh.Add(new Defenition
                                {
                                    Id = item.Key,
                                    Title = item.Value.Metadata.Title,
                                    Class = cls,
                                    Risk = "Высокий",
                                    Description = item.Value.Metadata.Description,
                                    Hyperlink = GetHyperlink(item.Value)
                                });
                                break;
                            case Enums.RiskLevel.Medium:
                                defenitionsMedium.Add(new Defenition
                                {
                                    Id = item.Key,
                                    Title = item.Value.Metadata.Title,
                                    Class = cls,
                                    Risk = "Средний",
                                    Description = item.Value.Metadata.Description,
                                    Hyperlink = GetHyperlink(item.Value)
                                });
                                break;
                            case Enums.RiskLevel.Low:
                                defenitionsLow.Add(new Defenition
                                {
                                    Id = item.Key,
                                    Title = item.Value.Metadata.Title,
                                    Class = cls,
                                    Risk = "Низкий",
                                    Description = item.Value.Metadata.Description,
                                    Hyperlink = GetHyperlink(item.Value)
                                });
                                break;
                        }
                    }
                }
                OvalInfoLoaded?.Invoke(this, $"Завершение...");
                StateChanged?.Invoke(this, ScapState.Result);
                GC.SuppressFinalize(_defenitionEvaluator);
                _defenitionEvaluator = null;
                GC.Collect();
            });
        }

        private void OnChanged(object? sender, OvalStatusEventArgs e)
        {
            if (e.Status == OvalStatusEventArgs.OvalStatus.Prepare) ;//not used
            else if (e.Status == OvalStatusEventArgs.OvalStatus.Validate)
            {

            }
            else if (e.Status == OvalStatusEventArgs.OvalStatus.CollectSystemData)
            {

            }
            else if (e.Status == OvalStatusEventArgs.OvalStatus.CreateCollectedObjects)
            {

            }
            else if (e.Status == OvalStatusEventArgs.OvalStatus.Evaluate)
            {

            }
            else if (e.Status == OvalStatusEventArgs.OvalStatus.GenerateOutput) ;//not used
        }

        private string? GetHyperlink(DefinitionType type)
        {
            string result = null;
            foreach (var reference in type.Metadata.References)
            {
                if (reference.RefSource.ToLower() == "fstec")
                    result = reference.RefUrl;
            }
            return result;
        }
    }
}
