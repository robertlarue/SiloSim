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

namespace SiloSim
{
    class Core
    {
        public static volatile bool hornResetEnabled = true;
        public static SiloSimForm form;
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
            while (true)
            {
                CheckGates();
                CheckMotion();
                if (form != null)
                {
                    form.SetText(string.Format("{0,7}", Scale.scaleWeight.ToString("D2")));
                    form.SetInputs(PLC.inputs);
                    form.SetOutputs(PLC.outputs);
                    form.SetBinInventory(PLC.binInventory);
                    form.SetScaleMotion(Scale.scaleInMotion);
                    form.SetClientLights(Scale.scaleClients.Count > 0, PLC.plcClients.Count > 0);
                }
                Thread.Sleep(10);
            }
        }
        static void CheckGates()
        {
            if (PLC.outputs[0] | PLC.outputs[1] | PLC.outputs[2] | PLC.outputs[3] | PLC.outputs[4] | PLC.outputs[5] | PLC.outputs[6] | PLC.outputs[7] | PLC.outputs[8] | PLC.outputs[9])
            {
                Scale.scaleWeight = Scale.scaleWeight + Scale.fillIncrement;
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