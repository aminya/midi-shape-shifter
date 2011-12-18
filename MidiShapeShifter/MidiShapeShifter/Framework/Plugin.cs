﻿using System.Diagnostics;

using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Framework.Plugin;

using MidiShapeShifter.Mss;
using MidiShapeShifter.Mss.Mapping;

namespace MidiShapeShifter.Framework
{
    /// <summary>
    /// The Plugin root object.
    /// </summary>
    public class Plugin : VstPluginWithInterfaceManagerBase
    {
        //TODO: Register an actual code for this
        private static readonly int UniquePluginId = new FourCharacterCode("1132").ToInt32();
        private static readonly string PluginName = "Midi Shape Shifter";
        private static readonly string ProductName = "Midi Shape Shifter";
        private static readonly string VendorName = "SpeqSoft";
        private static readonly int PluginVersion = 1;

        public MssComponentHub MssHub;
        protected VstParameters vstParameters;

        /// <summary>
        /// Initializes the one an only instance of the Plugin root object.
        /// </summary>
        public Plugin()
            : base(PluginName,
            new VstProductInfo(ProductName, VendorName, PluginVersion),
                VstPluginCategory.Effect,
                VstPluginCapabilities.ReceiveTimeInfo,
                // initial delay: number of samples your plugin lags behind.
                0,
                UniquePluginId)
        {
            this.MssHub = new MssComponentHub();
            this.MssHub.Init();

            this.vstParameters = new VstParameters();
            this.vstParameters.Init(this.MssHub.MssParameters, this.PluginPrograms);

            this.Opened += new System.EventHandler(Plugin_Opened);
        }

        /// <summary>
        /// Gets the audio processor object.
        /// </summary>
        public DummyAudioHandler AudioHandler
        {
            get { return GetInstance<DummyAudioHandler>(); }
        }

        /// <summary>
        /// Gets the midi processor object.
        /// </summary>
        public MidiHandler MidiHandler
        {
            get { return GetInstance<MidiHandler>(); }
        }

        /// <summary>
        /// Gets the plugin editor object.
        /// </summary>
        public PluginEditor PluginEditor
        {
            get { return GetInstance<PluginEditor>(); }
        }

        /// <summary>
        /// Gets the plugin programs object.
        /// </summary>
        public PluginPrograms PluginPrograms
        {
            get { return GetInstance<PluginPrograms>(); }
        }

        /// <summary>
        /// Implement this when you do audio processing.
        /// </summary>
        /// <param name="instance">A previous instance returned by this method. 
        /// When non-null, return a thread-safe version (or wrapper) for the object.</param>
        /// <returns>Returns null when not supported by the plugin.</returns>
        protected override IVstPluginAudioProcessor CreateAudioProcessor(IVstPluginAudioProcessor instance)
        {
            if (instance == null)
            {
                DummyAudioHandler newDummpAudioHandler = new DummyAudioHandler();
                newDummpAudioHandler.Init(this.MssHub.HostInfoInputPort);
                return newDummpAudioHandler;
            }

            return base.CreateAudioProcessor(instance); 
        }

        /// <summary>
        /// Implement this when you do midi processing.
        /// </summary>
        /// <param name="instance">A previous instance returned by this method. 
        /// When non-null, return a thread-safe version (or wrapper) for the object.</param>
        /// <returns>Returns null when not supported by the plugin.</returns>
        protected override IVstMidiProcessor CreateMidiProcessor(IVstMidiProcessor instance)
        {
            if (instance == null)
            {
                MidiHandler newMidiHandler = new MidiHandler();
                newMidiHandler.Init(this.MssHub.DryMssEventInputPort, this.MssHub.WetMssEventOutputPort, this.MssHub.HostInfoOutputPort);
                return newMidiHandler;
            }

            return base.CreateMidiProcessor(instance);
        }

        /// <summary>
        /// Creates a default instance and reuses that for all threads.
        /// </summary>
        /// <param name="instance">A reference to the default instance or null.</param>
        /// <returns>Returns the default instance.</returns>
        protected override IVstPluginPersistence CreatePersistence(IVstPluginPersistence instance)
        {
            if (instance == null)
            {
                VstPluginPersistence pluginPersistence = new VstPluginPersistence();
                pluginPersistence.Init(this);
                return pluginPersistence;
            }
            else
            {
                return instance;
            }
        }

        /// <summary>
        /// Implement this when you output midi events to the host.
        /// </summary>
        /// <param name="instance">A previous instance returned by this method. 
        /// When non-null, return a thread-safe version (or wrapper) for the object.</param>
        /// <returns>Returns null when not supported by the plugin.</returns>
        protected override IVstPluginMidiSource CreateMidiSource(IVstPluginMidiSource instance)
        {
            // we implement this interface on out midi processor.
            return (IVstPluginMidiSource)MidiHandler;
        }

        /// <summary>
        /// Implement this when you need a custom editor (UI).
        /// </summary>
        /// <param name="instance">A previous instance returned by this method. 
        /// When non-null, return a thread-safe version (or wrapper) for the object.</param>
        /// <returns>Returns null when not supported by the plugin.</returns>
        protected override IVstPluginEditor CreateEditor(IVstPluginEditor instance)
        {
            if (instance == null)
            {
                return new PluginEditor(this);
            }

            return base.CreateEditor(instance);
        }

        /// <summary>
        /// Implement this when you implement plugin programs or presets.
        /// </summary>
        /// <param name="instance">A previous instance returned by this method. 
        /// When non-null, return a thread-safe version (or wrapper) for the object.</param>
        /// <returns>Returns null when not supported by the plugin.</returns>
        protected override IVstPluginPrograms CreatePrograms(IVstPluginPrograms instance)
        {
            if (instance == null)
            {
                return new PluginPrograms();
            }

            return base.CreatePrograms(instance);
        }


        private void Plugin_Opened(object sender, System.EventArgs e)
        {
            //Anything associated with Plugin.Host must be set up in this even handler and not in the constructor 
            //becasue Plugin.Host is null when the constructor is called.
            Debug.Assert(this.Host != null);
            this.MidiHandler.InitVstHost(this.Host);
            this.AudioHandler.InitVstHost(this.Host);
            this.vstParameters.InitHostAutomation(this.Host);
        }
    }
}
