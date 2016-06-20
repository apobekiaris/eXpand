using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using ModelDifferenceTester.Module.FunctionalTests.ApplicationModel;
using ModelDifferenceTester.Module.FunctionalTests.RoleModel;
using ModelDifferenceTester.Module.FunctionalTests.UserModel;
using Xpand.ExpressApp.ModelDifference.Security;
using Xpand.ExpressApp.Security.Core;

namespace ModelDifferenceTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();

            var adminRole = ObjectSpace.GetAdminRole("Admin");
            adminRole.GetUser("Admin");

            var userRole = ObjectSpace.GetRole("User");
            var user = (SecuritySystemUser)userRole.GetUser("user");
            user.Roles.Add(defaultRole);
            user = (SecuritySystemUser)userRole.GetUser("user2");
            user.Roles.Add(defaultRole);
            userRole.EnsureTypePermissions<RoleModelObject>(SecurityOperations.FullAccess);
            userRole.EnsureTypePermissions<UserModelObject>(SecurityOperations.FullAccess);
            userRole.EnsureTypePermissions<ApplicationModelObject>(SecurityOperations.FullAccess);


            var modelRole = (SecuritySystemRole)ObjectSpace.GetDefaultModelRole("ModelRole");
            user.Roles.Add(modelRole);

            ObjectSpace.CommitChanges();
        }
    }
}
