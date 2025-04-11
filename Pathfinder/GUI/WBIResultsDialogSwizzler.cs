using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens.Flight.Dialogs;
using WBIResources;

/*
Source code copyrighgt 2015, by Michael Billard (Angel-125)
License: GPLV3

If you want to use this code, give me a shout on the KSP forums! :)
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WBIPathfinder
{
    public delegate bool OnTransmit(ScienceData data);
    public delegate bool OnDiscard(ScienceData data);
    public delegate bool OnProcess(ScienceData data);
    public delegate bool OnKeep(ScienceData data);

    public struct DialogCallbacks
    {
        public Callback<ScienceData> originalTransmitCallback;
        public Callback<ScienceData> originalDiscardCallback;
        public Callback<ScienceData> originalProcessCallback;
        public Callback<ScienceData> originalKeepCallback;
    }

    /* 
     * ExperimentResultDialogPage has public properties for its callbacks to transmit, keep, discard, and process its data.
     * We're going to swizzle those methods. :) This class gives you the ability to intercept the user events and call your own code before
     * letting the swizzled callback continue on its way- or not. For this to work properly, you'll need to hide ModuleScienceContainer's
     * ReviewDataEvent. Assuming you have a ModuleScienceContainer named myContainer, hide the event like so: myContainer.Events["ReviewDataEvent"].guiActive = false;
     * At this point, you supply your own event.
     * 
     * Here's how it works. In your own event handler for reviewing data:
     * 
     * 1. Using your instance of ModuleScienceContainer (ex: myContainer), call myContainer.ReviewData(). This will tell the container to
     * create an ExperimentResultDialogPage for each experiment data it has. 
     * 
     * 2. Create an instance of WBIResultsDialogSwizzler.
     * 
     * 3. Set the swizzler's delegates to your own implemented methods. None are required, so feel free to not implement delegates for callbacks you're not interested in.
     * 
     * 4. In your implemented method, return true if you want the swizzler to continue calling the original callback, or false if you don't.
     * 
     * For an example of how to use this class, see WBIGeologyLab in my Pathfinder mod.
     */
    public class WBIResultsDialogSwizzler
    {
        //Original callbacks
        Dictionary<ExperimentResultDialogPage, DialogCallbacks> callbacks = new Dictionary<ExperimentResultDialogPage, DialogCallbacks>();

        //Delegates
        public OnTransmit onTransmit;
        public OnDiscard onDiscard;
        public OnProcess onProcess;
        public OnKeep onKeep;

        #region Constructors
        public WBIResultsDialogSwizzler()
        {
        }

        public WBIResultsDialogSwizzler(OnTransmit transmitMethod, OnDiscard discardMethod, OnProcess processMethod, OnKeep keepMethod)
        {
            onTransmit = transmitMethod;
            onDiscard = discardMethod;
            onProcess = processMethod;
            onKeep = keepMethod;

            SwizzleResultsDialog();
        }
        #endregion

        #region API
        public void SwizzleResultsDialog()
        {
            callbacks.Clear();
            ExperimentsResultDialog dlg = ExperimentsResultDialog.Instance;

            //Swizzle the callbacks
            foreach (ExperimentResultDialogPage page in dlg.pages)
            {
                //Save the originals.
                DialogCallbacks dialogCallbacks = new DialogCallbacks();
                dialogCallbacks.originalTransmitCallback = page.OnTransmitData;
                dialogCallbacks.originalDiscardCallback = page.OnDiscardData;
                dialogCallbacks.originalProcessCallback = page.OnSendToLab;
                dialogCallbacks.originalKeepCallback = page.OnKeepData;
                callbacks.Add(page, dialogCallbacks);

                //Now add our own callbacks
                page.OnTransmitData = swizzleTransmit;
                page.OnDiscardData = swizzleDiscard;
                page.OnSendToLab = swizzleProcess;
                page.OnKeepData = swizzleKeep;
            }
        }

        public void Discard(ScienceData data)
        {
            ExperimentsResultDialog dlg = ExperimentsResultDialog.Instance;
            ExperimentResultDialogPage page = dlg.currentPage;
            DialogCallbacks dialogCallbacks;

            if (page == null)
                return;

            //Get the callbacks
            if (callbacks.ContainsKey(page) == false)
                return;
            dialogCallbacks = callbacks[page];

            //Original callback
            if (dialogCallbacks.originalDiscardCallback != null)
                dialogCallbacks.originalDiscardCallback(data);
        }

        public void Process(ScienceData data)
        {
            ExperimentsResultDialog dlg = ExperimentsResultDialog.Instance;
            ExperimentResultDialogPage page = dlg.currentPage;
            DialogCallbacks dialogCallbacks;

            if (page == null)
                return;

            //Get the callbacks
            if (callbacks.ContainsKey(page) == false)
                return;
            dialogCallbacks = callbacks[page];

            //Original callback
            if (dialogCallbacks.originalProcessCallback != null)
                dialogCallbacks.originalProcessCallback(data);
        }

        public void Keep(ScienceData data)
        {
            ExperimentsResultDialog dlg = ExperimentsResultDialog.Instance;
            ExperimentResultDialogPage page = dlg.currentPage;
            DialogCallbacks dialogCallbacks;

            if (page == null)
                return;

            //Get the callbacks
            if (callbacks.ContainsKey(page) == false)
                return;
            dialogCallbacks = callbacks[page];

            //Original callback
            if (dialogCallbacks.originalKeepCallback != null)
                dialogCallbacks.originalKeepCallback(data);
        }

        public void Transmit(ScienceData data)
        {
            ExperimentsResultDialog dlg = ExperimentsResultDialog.Instance;
            ExperimentResultDialogPage page = dlg.currentPage;
            DialogCallbacks dialogCallbacks;

            if (page == null)
                return;

            //Get the callbacks
            if (callbacks.ContainsKey(page) == false)
                return;
            dialogCallbacks = callbacks[page];

            //Original callback
            if (dialogCallbacks.originalTransmitCallback != null)
                dialogCallbacks.originalTransmitCallback(data);
        }
        #endregion

        #region Swizzle Methods
        protected void swizzleDiscard(ScienceData data)
        {
            //Call the delegate
            if (onDiscard != null)
            {
                if (onDiscard(data) == false)
                    return;
            }

            Discard(data);
        }

        protected void swizzleProcess(ScienceData data)
        {
            //Call the delegate
            if (onProcess != null)
            {
                if (onProcess(data) == false)
                    return;
            }

            Process(data);
        }

        protected void swizzleKeep(ScienceData data)
        {
            //Call the delegate
            if (onKeep != null)
            {
                if (onKeep(data) == false)
                    return;
            }

            Keep(data);
        }

        protected void swizzleTransmit(ScienceData data)
        {
            //Call the delegate
            if (onTransmit != null)
            {
                if (onTransmit(data) == false)
                    return;
            }

            Transmit(data);
        }
        #endregion
    }
}
