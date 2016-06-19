using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.FileAttachments.Win;
using DevExpress.Utils;
using Xpand.ExpressApp.Win.PropertyEditors;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;
using Xpand.ExpressApp.Win.SystemModule;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using AssemblyHelper = DevExpress.ExpressApp.Utils.Reflection.AssemblyHelper;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.WorldCreator.Win {
    [ToolboxBitmap(typeof(WorldCreatorWinModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class WorldCreatorWinModule : WorldCreatorModuleBase, IRichEditUser {
        
        public WorldCreatorWinModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            RequiredModuleTypes.Add(typeof(Security.Win.XpandSecurityWinModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsWindowsFormsModule));
        }

        protected override IEnumerable<Type> GetRegularTypes(){
            var richEditTypes = AssemblyHelper.GetTypesFromAssembly(typeof(XpandSystemWindowsFormsModule).Assembly)
                    .Where(type => type.Namespace != null && type.Namespace.Contains("RichEdit"));
            return base.GetRegularTypes().Concat(richEditTypes);
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.CSCodePropertyEditor, typeof(string), typeof(CSCodePropertyEditor), false)));
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes() {
            return base.GetDeclaredControllerTypes().Concat(new[] { typeof(RichEditModelAdapterController) });
        }

        public override string GetPath() {
            return Application.GetStorageFolder(WCAssembliesPath);
        }
    }
}