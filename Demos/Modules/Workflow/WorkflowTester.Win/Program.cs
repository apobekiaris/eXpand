using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Workflow.Versioning;
using DevExpress.ExpressApp.Workflow.Xpo;
using WorkflowTester.Module;
using WorkflowTester.Module.Win;
using Xpand.ExpressApp.Workflow;
using Xpand.Persistent.Base.General;

namespace WorkflowTester.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(){
            

#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = true;
            var winApplication = new WorkflowTesterWindowsFormsApplication();
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }

#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif

            WorkflowServerStarter workflowServerStarter = null;
            if (ApplicationHelper.Instance.Application.GetEasyTestParameter("WorldCreator")){
                winApplication.LoggedOn += delegate{
                    if (workflowServerStarter == null){
                        workflowServerStarter = new WorkflowServerStarter();
                        workflowServerStarter.OnCustomHandleException += delegate(object sender1, ExceptionEventArgs args1) {
                            MessageBox.Show(args1.Message);
                        };
                        workflowServerStarter.Start<XpoWorkflowDefinition, XpoUserActivityVersion,WorkflowTesterWindowsFormsModule>(winApplication);
                    }
                };
            }

            try {
                winApplication.UseOldTemplates=false;
                winApplication.ProjectSetup();
                winApplication.Setup();
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
            workflowServerStarter?.Stop();
        }
    }

    public class WorkflowServerStarter : Xpand.ExpressApp.Workflow.WorkflowServerStarter {
        protected override ServerApplication GetServerApplication(){
            return new ServerApp();
        }

        class ServerApp:ServerApplication{
        }
    }
}
