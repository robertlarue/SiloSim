using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SiloSim
{
    class Core
    {
        public static volatile bool hornResetEnabled = true;
        public static bool startWeighBridgeOnStartup = false;
        public static int weighBridgeStartupFillRate = 300;
        public static int inAirWeight = 2000;
        public static float materialInAir = 0F;
        public static int loadoutRate = 5000;
        public static float materialVelocity = 0.1F;
        public static float frictionFactor = 0.5F;
        public static float distanceToTruck = 8F;
        public static float materialStreamHeight = 8F;
        public static float siloOpeningArea = 4F;
        public static float materialDensity = 145F;
        public const float gravity = 32.174F;
        public static float gatePercentOpen = 0F;
        public static SiloSimForm form;
        public static Queue<float> materialInAirQueue = new Queue<float>(500);

        //public static Stopwatch sw = new Stopwatch();

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                SiloSimForm form = new SiloSimForm();
                int scalePort = 0;
                if (int.TryParse(args[0], out scalePort))
                {
                    if (form.validatePort(args[0]))
                    {
                        SiloSim.Scale.port = scalePort;
                    }
                }
                int plcPort = 0;
                if (int.TryParse(args[1], out plcPort))
                {
                    if (form.validatePort(args[0]))
                    {
                        SiloSim.PLC.port = plcPort;
                    }
                }
            }
            if (args.Length > 2)
            {
                SiloSimForm.label = args[2];
            }
            if (args.Length > 3)
            {
                if (int.TryParse(args[3], out int selectedLoadoutSilo))
                {
                    switch (selectedLoadoutSilo)
                    {
                        case 1:
                            PLC.inputs[4] = true;
                            break;
                        case 2:
                            PLC.inputs[5] = true;
                            break;
                        case 3:
                            PLC.inputs[6] = true;
                            break;
                        case 4:
                            PLC.inputs[7] = true;
                            break;
                        case 5:
                            PLC.inputs[8] = true;
                            break;
                    }
                }
            }
            if (args.Length > 4)
            {
                if (int.TryParse(args[4], out int selectedFillSilo))
                {
                    switch (selectedFillSilo)
                    {
                        case 1:
                            PLC.inputs[15] = true;
                            break;
                        case 2:
                            PLC.inputs[16] = true;
                            break;
                        case 3:
                            PLC.inputs[17] = true;
                            break;
                        case 4:
                            PLC.inputs[18] = true;
                            break;
                        case 5:
                            PLC.inputs[19] = true;
                            break;
                    }
                }
            }
            if (args.Length > 5)
            {
                if (int.TryParse(args[5], out int fillRate))
                {
                    startWeighBridgeOnStartup = true;
                    weighBridgeStartupFillRate = fillRate;
                }
            }
            Thread scaleThread = new Thread(Scale.ListenClientThread);
            scaleThread.Start(SiloSim.Scale.port);
            Thread plcThread = new Thread(PLC.ListenClientThread);
            plcThread.Start(PLC.port);
            Thread coreThread = new Thread(Core.CoreThread);
            coreThread.Start();
            Application.Run(form = new SiloSimForm());
        }
        static void CoreThread()
        {
            ushort iteration = 0;
            while (true)
            {
                CheckGates();
                CheckMotion();
                if (form != null && iteration > 8)
                {
                    form.SetText(string.Format("{0,7}", Scale.scaleWeight.ToString("D2")));
                    form.SetInputs(PLC.inputs);
                    form.SetOutputs(PLC.outputs);
                    form.SetBinInventory(PLC.binInventory);
                    form.SetScaleMotion(Scale.scaleInMotion);
                    form.SetClientLights(Scale.scaleClients.Count > 0, PLC.plcClients.Count > 0);
                    if (startWeighBridgeOnStartup)
                    {
                        form.StartWeighBridge(weighBridgeStartupFillRate);
                        startWeighBridgeOnStartup = false;
                    }
                    iteration = 0;
                }
                iteration++;
                Thread.Sleep(10);
            }
        }
        static void CheckGates()
        {
            if (PLC.outputs[0] | PLC.outputs[1] | PLC.outputs[2] | PLC.outputs[3] | PLC.outputs[4] | PLC.outputs[5] | PLC.outputs[6] | PLC.outputs[7] | PLC.outputs[8] | PLC.outputs[9])
            {
                if (gatePercentOpen < 100)
                {
                    gatePercentOpen += 4;
                }
                else
                {
                    gatePercentOpen = 100;
                }
            }
            else
            {
                if (gatePercentOpen > 0)
                {
                    gatePercentOpen -= 4;
                }
                else
                {
                    gatePercentOpen = 0;
                }
            }

            if(materialInAir > inAirWeight)
            {
                materialInAir = inAirWeight;
            }


            if (materialStreamHeight > 0)
            {
                if (materialInAir < inAirWeight)
                {
                    materialVelocity += gravity / 100;
                }
                materialStreamHeight -= materialVelocity / 100;
            }
            else
            {
                materialStreamHeight = 0;
            }

            if (gatePercentOpen > 0)
            {
                var material = gatePercentOpen / 100 * materialVelocity * siloOpeningArea * materialDensity / 100;
                materialInAirQueue.Enqueue(material);
                materialInAir += gatePercentOpen / 100 * materialVelocity * siloOpeningArea * materialDensity / 100;
            }

            if (materialStreamHeight <= 0 && materialInAir > 0)
            {
                var materialLandedInTruck = materialInAirQueue.Dequeue();
                materialInAir -= materialLandedInTruck;
                Scale.scaleWeight += (int)Math.Round(materialLandedInTruck);
            }

            if(gatePercentOpen > 0 || materialInAir > 0)
            {
                Debug.WriteLine("materialVelocity={0},materialInAir={1},materialStreamHeight={2},loadoutRate={3},gatePercentOpen={4}", materialVelocity, materialInAir, materialStreamHeight,materialVelocity*siloOpeningArea*materialDensity,gatePercentOpen);
            }
            else
            {
                materialInAir = 0;
                materialVelocity = 0.1F;
                materialStreamHeight = distanceToTruck;
            }

            if (hornResetEnabled)
            {
                if (PLC.outputs[11])
                {
                    Scale.scaleWeight = Scale.tareWeight;
                }
            }
            if (PLC.outputs[11])
            {
                form.SoundHorn();
            }
        }
        static void CheckMotion()
        {
            if (Scale.prevScaleWeights.Count > 0 && Scale.scaleWeight != Scale.prevScaleWeights.Average())
            {
                Scale.scaleInMotion = true;
            }
            else
            {
                Scale.scaleInMotion = false;
            }
            Scale.prevScaleWeights.Enqueue(Scale.scaleWeight);
            if (Scale.prevScaleWeights.Count > 10)
            {
                Scale.prevScaleWeights.Dequeue();
            }
        }
    }
}