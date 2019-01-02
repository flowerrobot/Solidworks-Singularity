using NLog;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorksTools;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using SingularityBase;
using SingularityCore;

namespace SingularityAddin
{
    [Guid("b6fb78e7-dda9-4638-b868-bd25a24de56e"), ComVisible(true)]
    [SwAddin(
        Description = "Singularity Addin for simplier addons, allowing plugins to just work, with a sw wrapper",
        Title = "Singularity Addin",
        LoadAtStartup = true
        )]
    public class SwAddin : ISwAddin
    {
        public static Logger Logger { get; } = NLog.LogManager.GetLogger("SWAddin");

        #region Local Variables                
        private int addinID = 0;
        private int registerID;


        SingularityCore.SingleCommandMgr SwApp { get; set; }

        #endregion

        #region SolidWorks Registration
        [ComRegisterFunctionAttribute]
        public static void RegisterFunction(Type t)
        {
            #region Get Custom Attribute: SwAddinAttribute
            SwAddinAttribute SWattr = null;
            Type type = typeof(SwAddin);

            foreach (System.Attribute attr in type.GetCustomAttributes(false))
            {
                if (attr is SwAddinAttribute)
                {
                    SWattr = attr as SwAddinAttribute;
                    break;
                }
            }

            #endregion

            try
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                Microsoft.Win32.RegistryKey addinkey = hklm.CreateSubKey(keyname);
                addinkey.SetValue(null, 0);

                addinkey.SetValue("Description", SWattr.Description);
                addinkey.SetValue("Title", SWattr.Title);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                addinkey = hkcu.CreateSubKey(keyname);
                addinkey.SetValue(null, Convert.ToInt32(SWattr.LoadAtStartup), Microsoft.Win32.RegistryValueKind.DWord);
            }
            catch (System.NullReferenceException nl)
            {
                Console.WriteLine("There was a problem registering this dll: SWattr is null. \n\"" + nl.Message + "\"");
                Logger.Error(nl,"There was a problem registering this dll: SWattr is null.\n\"" + nl.Message + "\"");
            }

            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);

                Logger.Error(e,"There was a problem registering the function: \n\"" + e.Message + "\"");
            }
        }

        [ComUnregisterFunctionAttribute]
        public static void UnregisterFunction(Type t)
        {
            try
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                hklm.DeleteSubKey(keyname);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                hkcu.DeleteSubKey(keyname);
            }
            catch (System.NullReferenceException nl)
            {
                Console.WriteLine("There was a problem unregistering this dll: " + nl.Message);
                Logger.Error(nl,"There was a problem unregistering this dll: \n\"" + nl.Message + "\"");
            }
            catch (System.Exception e)
            {
                Console.WriteLine("There was a problem unregistering this dll: " + e.Message);
                Logger.Error(e,"There was a problem unregistering this dll: \n\"" + e.Message + "\"");
            }
        }

        #endregion

        #region ISwAddin Implementation
        public SwAddin()
        {
            //TODO updater?
            Logger.Trace("Singularity Starting");
        }

        public bool ConnectToSW(object ThisSW, int cookie)
        {
            Logger.Trace("Singularity Starting to connect to SW");
            SwApp = new SingularityCore.SingleCommandMgr((ISldWorks)ThisSW, cookie);
            addinID = cookie;
            return true;
        }

        public bool DisconnectFromSW()
        {
            Logger.Trace("Singularity disconnecting to SW");
            SwApp.DisconnectFromSw();            
                        
            //The addin _must_ call GC.Collect() here in order to retrieve all managed code pointers 
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return true;
        }
        
        #endregion

    }
}
