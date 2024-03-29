﻿using System;
using System.Collections.Generic;
using System.Text;
using KidouCS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KidouCSTest
{
    internal class Program
    {
        // zentrale Instanz für die Microphon Steuerung
        private static KidouMicrophoneHandler mic = null;
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Demo Application");

            LadeModelle();

            LadeKommandos();
            
            LadeDrugs();
            
            /* Überprüfe die Initialisierung
             * 
             */
            if (KidouModelHandler.CheckModelsLoaded())
            {
                Console.WriteLine("Done Loading Models");
            }
            else
            {
                Console.WriteLine("Something gone wrong with loading models");
                return;
            }

            /* Initialer Zustand
             * 1stes Microphone selectiert aber inaktiv
             * alle Modelle inaktiv
             */
            KidouMicrophoneHandler.KidouDataAvailable += KidouModelHandler.MicDataAvailable;
            KidouMicrophoneHandler.PrintDeviceListToConsole();
            List<string> microphones = KidouMicrophoneHandler.EnumerateDeviceNames();
            KidouMicrophoneHandler.SelectMicrophone(microphones[0]);
            Console.WriteLine("Selected Microphone No. 1 for you:" + microphones[0]);
            
            KidouModelHandler.ChangeModelActivationStatus(1, false);
            KidouModelHandler.ChangeModelActivationStatus(2, false);
            KidouModelHandler.ChangeModelActivationStatus(3, false);

            Console.WriteLine("No model active, toggle for activation");
            
            /* Handler für erkannte Texte setzen
             */
            KidouModelHandler.configureKidouModelTextEventHandler(KidouModelTextEventAvailable);

            StarteMenue();
        }
        
        /* Lädt die Modelle
         * VAD
         * STT
         * LM mit CTC
         * Greedy Language Modell
         */
        public static void LadeModelle()
        {
            KidouModelHandler.DN_init(".\\models\\dn\\de-normalization-2020-04-15.pt", 
                ".\\models\\dn\\vocab-electra.txt");
            
            KidouModelHandler.VAD_init(".\\models\\vad\\vad.jit");
            
            KidouModelHandler.STT_init_withmodelnumber(1,
                ".\\models\\stt1\\pre-cpu.ts",
                ".\\models\\stt1\\enc-cpu.ts",
                ".\\models\\stt1\\dec-cpu.ts",
                ".\\models\\stt1\\voc.txt");
            
            KidouModelHandler.STT_init_withmodelnumber(2,
                ".\\models\\stt1\\pre-cpu.ts",
                ".\\models\\stt1\\enc-cpu.ts",
                ".\\models\\stt1\\dec-cpu.ts",
                ".\\models\\stt1\\voc.txt");
            
            KidouModelHandler.STT_init_withmodelnumber(3,
                ".\\models\\stt2\\pre-cpu.ts",
                ".\\models\\stt2\\enc-cpu.ts",
                ".\\models\\stt2\\dec-cpu.ts",
                ".\\models\\stt2\\voc.txt");            
            
            KidouModelHandler.LM_CTC_init_withmodelnumber(1,
                ".\\models\\lm1\\7GB-text-trigram-lower-pruned.bin", 
                ".\\models\\lm1\\voc.txt");

            KidouModelHandler.LM_CTC_init_withmodelnumber(2,
                ".\\models\\lm2\\trumpf-commands.binary",
                ".\\models\\lm2\\voc.txt");
            
            KidouModelHandler.LM_Greedy_init_withmodelnumber(3);
        }

        /* Lädt die MedikamentenListe */
        public static void LadeDrugs()
        {
            String drugs_json = @"
            { ""drugs"":
              [
            {""name"": ""paracetamol"", ""id"":1},
            {""name"": ""aspirin"", ""id"":2},
            {""name"": ""adrenalin"", ""id"":3},
            {""name"": ""akrinor"", ""id"":4},
            {""name"": ""atropin"", ""id"":5},
            {""name"": ""dobutamin"", ""id"":6},
            {""name"": ""naloxon"", ""id"":7},
            {""name"": ""lidocain"", ""id"":8},

            {""name"": ""diazepam"", ""id"":9},
            {""name"": ""valium"", ""id"":9},

            {""name"": ""epinephrin"", ""id"":10},
            {""name"": ""adrenalin"", ""id"":10}
            ],
            ""forms"":
            [
            {""name"": ""oral"", ""id"":1},
            {""name"": ""intravenös"", ""id"":2},
            {""name"": ""lokal"", ""id"":3},
            {""name"": ""parental"", ""id"":4}
            ],
            ""units"":
            [
            {""name"": ""milliliter"", ""id"":1},
            {""name"": ""liter"", ""id"":2},
            {""name"": ""gramm"", ""id"":3},
            {""name"": ""mikrogramm"", ""id"":4},
            {""name"": ""milligramm"", ""id"":5}
            ]
        }
            ";
            EM_Native.init_drugs(drugs_json);
        }
        /* Lädt die KommandoListe */
        public static void LadeKommandos()
        {
            String commands_json = @"[
            {""text"": ""bitte helfen"", ""id"":100},
            {""text"": ""hilf mir"", ""id"":100},
            {""text"": ""zurück im menü"", ""id"":100},
            
            {""id"":1, ""text"": ""ein teil defekt""},
            {""id"":2, ""text"": ""zwei teile defekt""},
            {""id"":3, ""text"": ""drei teile defekt""},
            {""id"":4, ""text"": ""vier teile defekt""},
            {""id"":5, ""text"": ""fünf teile defekt""},
            {""id"":6, ""text"": ""sechs teile defekt""},
            {""id"":7, ""text"": ""sieben teile defekt""},
            {""id"":8, ""text"": ""acht teile defekt""},
            {""id"":9, ""text"": ""neun teile defekt""},
            {""id"":10, ""text"": ""zehn teile defekt""},
            {""id"":11, ""text"": ""ein teil mehr""},
            {""id"":12, ""text"": ""zwei teile mehr""},
            {""id"":13, ""text"": ""drei teile mehr""},
            {""id"":14, ""text"": ""vier teile mehr""},
            {""id"":15, ""text"": ""fünf teile mehr""},
            {""id"":16, ""text"": ""sechs teile mehr""},
            {""id"":17, ""text"": ""sieben teile mehr""},
            {""id"":18, ""text"": ""acht teile mehr""},
            {""id"":19, ""text"": ""neun teile mehr""},
            {""id"":20, ""text"": ""zehn teile mehr""},
            {""id"":21, ""text"": ""ein teil weniger""},
            {""id"":22, ""text"": ""zwei teile weniger""},
            {""id"":23, ""text"": ""drei teile weniger""},
            {""id"":24, ""text"": ""vier teile weniger""},
            {""id"":25, ""text"": ""fünf teile weniger""},
            {""id"":26, ""text"": ""sechs teile weniger""},
            {""id"":27, ""text"": ""sieben teile weniger""},
            {""id"":28, ""text"": ""acht teile weniger""},
            {""id"":29, ""text"": ""neun teile weniger""},
            {""id"":30, ""text"": ""zehn teile weniger""},
            {""id"":31, ""text"": ""bestand korrigieren auf ein teil""},
            {""id"":32, ""text"": ""bestand korrigieren auf zwei teile""},
            {""id"":33, ""text"": ""bestand korrigieren auf drei teile""},
            {""id"":34, ""text"": ""bestand korrigieren auf vier teile""},
            {""id"":35, ""text"": ""bestand korrigieren auf fünf teile""},
            {""id"":36, ""text"": ""bestand korrigieren auf sechs teile""},
            {""id"":37, ""text"": ""bestand korrigieren auf sieben teile""},
            {""id"":38, ""text"": ""bestand korrigieren auf acht teile""},
            {""id"":39, ""text"": ""bestand korrigieren auf neun teile""},
            {""id"":40, ""text"": ""bestand korrigieren auf zehn teile""},
            {""id"":41, ""text"": ""alle aufträge lösen""},
            {""id"":42, ""text"": ""neuer bestand mit einem teil""},
            {""id"":43, ""text"": ""neuer bestand mit zwei teilen""},
            {""id"":44, ""text"": ""neuer bestand mit drei teilen""},
            {""id"":45, ""text"": ""neuer bestand mit vier teilen""},
            {""id"":46, ""text"": ""neuer bestand mit fünf teilen""},
            {""id"":47, ""text"": ""neuer bestand mit sechs teilen""},
            {""id"":48, ""text"": ""neuer bestand mit sieben teilen""},
            {""id"":49, ""text"": ""neuer bestand mit acht teilen""},
            {""id"":50, ""text"": ""neuer bestand mit neun teilen""},
            {""id"":51, ""text"": ""neuer bestand mit zehn teilen""},

            {""id"":0, ""text"": ""an den anfang""}
            ]";
            
            EM_Native.init_commands(commands_json);
        }
        
        /* Lädt das Menü */
        public static void StarteMenue()
        {
            /* Consolen Menü
             * und Auswertung
             * Endlosschleife
             */
            while (true)
            {
                Console.WriteLine("Press [E]=End, [1|2|3] Toggle Models, [S] Start,[T] Stop Microphone Listener, [M] show microphone list, [M1,M2, ... MX] select microphone");
                string prompt = Console.ReadLine();
                if (prompt == null || prompt == "")
                {
                    continue;
                }
                
                prompt = prompt.ToUpper();
                
                // Abbruch
                if (prompt == "E")
                {
                    return;
                }

                // Model 1 togglen
                if (prompt == "1")
                {
                    Console.WriteLine("[1] Model activated");
                    KidouModelHandler.ChangeModelActivationStatus(1, true);
                    KidouModelHandler.ChangeModelActivationStatus(2, false);
                    KidouModelHandler.ChangeModelActivationStatus(3, false);
                }

                // Model 2 togglen
                if (prompt == "2")
                {
                    Console.WriteLine("[2] Model activated");
                    KidouModelHandler.ChangeModelActivationStatus(1, false);
                    KidouModelHandler.ChangeModelActivationStatus(2, true);
                    KidouModelHandler.ChangeModelActivationStatus(3, false);
                }

                // Model 3 togglen
                if (prompt == "3")
                {
                    Console.WriteLine("[3] Model activated");
                    KidouModelHandler.ChangeModelActivationStatus(1, false);
                    KidouModelHandler.ChangeModelActivationStatus(2, false);
                    KidouModelHandler.ChangeModelActivationStatus(3, true);
                }

                // Microphon Liste
                if (prompt == "M")
                {
                    /*
                     * Reloads the List of Microphones and print them out
                     */
                    KidouMicrophoneHandler.ReloadDevices();
                    KidouMicrophoneHandler.PrintDeviceListToConsole();
                }

                // Microphon Auswahl
                if (prompt.StartsWith("M") && prompt.Length > 1)
                {
                    /*
                     * Stops the current Microphone and starts a new Microphone identified by deviceNumber.
                     */
                    string deviceNumberString = prompt.Substring(1);
                    try
                    {
                        int deviceNumber = Int32.Parse(deviceNumberString);
                        KidouMicrophoneHandler.StopMicrophoneListener();
                        List<string> microphones = KidouMicrophoneHandler.EnumerateDeviceNames();
                        KidouMicrophoneHandler.SelectMicrophone(microphones[deviceNumber]);
                        KidouMicrophoneHandler.StartMicrophoneListener();
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"{deviceNumberString} is not a deviceNumber" );
                    }
                }
                
                // Aktivierung
                if (prompt.ToUpper() == "S")
                    {
                        Console.WriteLine("[S] Microphone activated now");
                        KidouMicrophoneHandler.StartMicrophoneListener();
                    }
                
                // Stop the Microphone Listener
                if (prompt.ToUpper() == "T")
                    {
                        Console.WriteLine("[T] Microphone deactivated now");
                        KidouMicrophoneHandler.StopMicrophoneListener();
                    }

            }
        }
        
        /* Beispiel für eine Auswertung der erkannten Texte
         */
        public static void KidouModelTextEventAvailable(object s, KidouModelTextEventArgs e)
        {
            // Variante dass man immer den Text zurückgibt (Freitext Diktat in Felder)
            Console.WriteLine("Model {0} returned: {1}", e.ModelNumber, e.DetectedText);

            if (e.ModelNumber == 1)
            {
                var postprocessed = EM_Native.replace_all_numbers(e.DetectedText).AsString();
                if (postprocessed != e.DetectedText)
                {
                    Console.WriteLine("Mit Nummernersetzung: {0}", postprocessed);
                }
                var tmp = DN_Native.denormalize(postprocessed).AsString();
                Console.WriteLine("Denormaliziert: {0}", tmp);
                
            }
            
            // Beim Kommando Model auch nach Kommando abgleichen und nach Konfidenz filtern
            if (e.ModelNumber == 2)
            {
                String command = EM_Native.check_command(e.DetectedText).AsString();
                
                var jsondata = (JObject) JsonConvert.DeserializeObject(command);
                var confidence = jsondata["score"].Value<double>();

                if (confidence > 0.7)
                {
                    Console.WriteLine("Kommando erkannt: {0}", command); 
                }
                else
                {
                    Console.WriteLine("Unsicher. Wiederhole. (höchstwahrscheinlich: {0}", command);
                }
            }

            if (e.ModelNumber == 3)
            {
                String drug = EM_Native.check_drug(e.DetectedText).AsString();
                var jsondata = (JObject) JsonConvert.DeserializeObject(drug);
                var drug_id = jsondata["drug_id"].Value<int>();
                if (drug_id != 0)
                {
                    Console.WriteLine("Medikament erkannt: {0}", drug);
                }
            }
                

        }
    }
}