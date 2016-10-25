﻿using System;
using System.Collections.Generic;
﻿using System.Linq;
﻿using System.Security;
﻿using System.Security.Permissions;
﻿using DevExpress.Data.Filtering;
﻿using DevExpress.ExpressApp;
﻿using DevExpress.ExpressApp.DC;
﻿using DevExpress.ExpressApp.Security;
﻿using DevExpress.ExpressApp.Security.Strategy;
﻿using DevExpress.ExpressApp.Xpo;
﻿using DevExpress.Persistent.Base;
﻿using DevExpress.Persistent.Base.Security;
﻿using DevExpress.Xpo;
﻿using Fasterflect;
﻿using Xpand.ExpressApp.Security.AuthenticationProviders;
﻿using Xpand.ExpressApp.Security.Permissions;
﻿using Xpand.Persistent.Base.General;
﻿using Xpand.Utils.Helpers;
﻿using IOperationPermissionProvider = DevExpress.ExpressApp.Security.IOperationPermissionProvider;

namespace Xpand.ExpressApp.Security.Core {
    [AttributeUsage(AttributeTargets.Class)]
    public class FullPermissionAttribute : Attribute {
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class PermissionBehaviorAttribute : Attribute {
        public PermissionBehaviorAttribute(object @enum) {
            if (!@enum.GetType().IsEnum)
                throw new NotImplementedException();
            Name = Enum.GetName(@enum.GetType(), @enum);
        }

        public PermissionBehaviorAttribute(string name) {
            Name = name;
        }

        public string Name { get; }
    }
    public static class SecuritySystemExtensions {

        public static void NewSecurityStrategyComplex<TAuthentation, TLogonParameter>(this XafApplication application, Type userType = null, Type roleType = null)
            where TAuthentation : AuthenticationBase {
                application.NewSecurityStrategyComplex(typeof(TAuthentation), typeof(TLogonParameter), userType ?? typeof(XpandUser),roleType??typeof(XpandRole));
        }

        public static void NewSecurityStrategyComplexV2<TUser,TRole>(this XafApplication application, Type authethicationType = null,
            Type logonParametersType = null) where TRole:ISecurityRole where TUser:ISecurityUser{
            NewSecurityStrategyComplexCore(application, authethicationType, logonParametersType, null, null, typeof(TUser), typeof(TRole));
        }

        public static void NewSecurityStrategyComplex(this XafApplication application,Type authethicationType=null, Type logonParametersType=null,Type userType=null,Type roleType=null){
            NewSecurityStrategyComplexCore(application, authethicationType, logonParametersType, userType, roleType, typeof(XpandUser), typeof(XpandRole));
        }

        private static void NewSecurityStrategyComplexCore(XafApplication application, Type authethicationType,
            Type logonParametersType, Type userType, Type roleType, Type defaultUserType, Type defaultRoleType){
            logonParametersType = logonParametersType ?? typeof(XpandLogonParameters);
            userType = userType ?? defaultUserType;
            AuthenticationStandard authenticationStandard = new XpandAuthenticationStandard(userType, logonParametersType);
            if (authethicationType != null){
                authenticationStandard = (AuthenticationStandard) authethicationType.CreateInstance();
                authenticationStandard.UserType = userType;
                authenticationStandard.LogonParametersType = logonParametersType;
            }
            var security = new SecurityStrategyComplex(userType, roleType ?? defaultRoleType, authenticationStandard);
            application.Security = security;
        }

        public static ISecurityRole GetDefaultRole(this IObjectSpace objectSpace, string roleName) {
            var defaultRole = objectSpace.GetRole(roleName);
            if (objectSpace.IsNewObject(defaultRole)) {
                var securitySystemRoleBase = defaultRole as SecuritySystemRoleBase;
                if (securitySystemRoleBase != null){
                    securitySystemRoleBase.AddObjectAccessPermission(SecuritySystem.UserType, "[Oid] = CurrentUserId()",SecurityOperations.ReadOnlyAccess);
                    securitySystemRoleBase.AddMemberAccessPermission(SecuritySystem.UserType,"ChangePasswordOnFirstLogon,StoredPassword", SecurityOperations.Write, "[Oid] = CurrentUserId()");
                    securitySystemRoleBase.GrandObjectAccessRecursively();
                }
                else{
                    var permissionPolicyRole = defaultRole as IPermissionPolicyRole;
                    permissionPolicyRole.AddObjectPermission(SecuritySystem.UserType,SecurityOperations.ReadOnlyAccess, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                    permissionPolicyRole.AddMemberPermission(SecuritySystem.UserType,SecurityOperations.ReadWriteAccess, "ChangePasswordOnFirstLogon; StoredPassword", null, SecurityPermissionState.Allow);
                }
            }
            return defaultRole;
        }

        public static void GrandObjectAccessRecursively(this SecuritySystemRoleBase defaultRole) {
            Type roleType=defaultRole.GetType();
            foreach (Type type in SecurityStrategy.GetSecuredTypes().Where(type => roleType == type || type.IsAssignableFrom(roleType))) {
                defaultRole.AddObjectAccessPermission(type, "[Name]='" + defaultRole.Name + "'", SecurityOperations.ReadOnlyAccess);
            }
        }

        public static ISecurityRole GetDefaultRole(this IObjectSpace objectSpace) {
            return objectSpace.GetDefaultRole("Default");
        }

        public static DelayedIPermissionDictionary WithSecurityOperationAttributePermissions(this IPermissionDictionary permissionDictionary){
            var permissions = ((ISecurityUserWithRoles)SecuritySystem.CurrentUser).Roles.OfType<IXpandRoleCustomPermissions>().SelectMany(role => role.SecurityOperationAttributePermissions());
            return new DelayedIPermissionDictionary(permissionDictionary.GetPermissions<IOperationPermission>().Concat(permissions));
        }

        public static DelayedIPermissionDictionary WithHiddenNavigationItemPermissions(this IPermissionDictionary permissionDictionary){
            var permissions = ((ISecurityUserWithRoles)SecuritySystem.CurrentUser).Roles.OfType<ISupportHiddenNavigationItems>().SelectMany(role => role.GetHiddenNavigationItemPermissions());
            return new DelayedIPermissionDictionary(permissionDictionary.GetPermissions<IOperationPermission>().Concat(permissions));
        }

        public static DelayedIPermissionDictionary WithCustomPermissions(this IPermissionDictionary permissionDictionary) {
            var permissions = ((ISecurityUserWithRoles)SecuritySystem.CurrentUser).Roles.OfType<IXpandRoleCustomPermissions>().SelectMany(role => role.GetCustomPermissions());
            return new DelayedIPermissionDictionary(permissionDictionary.GetPermissions<IOperationPermission>().Concat(permissions));
        }

        public static ISecurityUserWithRoles GetAnonymousUser(this XpandRole systemRole) {
            var optionsAthentication = ((IModelOptionsAuthentication)ApplicationHelper.Instance.Application.Model.Options).Athentication;
            var anonymousUserName = optionsAthentication.AnonymousAuthentication.AnonymousUser;
            return GetAnonymousUser(systemRole, anonymousUserName);
        }

        public static ISecurityUserWithRoles GetAnonymousUser(this XpandRole systemRole, string userName) {
            return systemRole.GetUser(userName);
        }

        public static ISecurityUserWithRoles GetUser(this ISecurityRole systemRole, string userName, string passWord = "") {
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(systemRole);
            return GetUser(objectSpace, userName, passWord, systemRole);
        }

        public static ISecurityUserWithRoles GetUser(this IObjectSpace objectSpace, string userName, string passWord = "", params ISecurityRole[] roles) {
            return (ISecurityUserWithRoles)objectSpace.FindObject(SecuritySystem.UserType, new BinaryOperator("UserName", userName)) ??
                        CreateUser(objectSpace, userName, passWord, roles);
        }

        public static ISecurityUserWithRoles CreateUser(IObjectSpace objectSpace, string userName, string passWord, ISecurityRole[] roles) {
            var user2 = (ISecurityUserWithRoles)objectSpace.CreateObject(SecuritySystem.UserType);
            var typeInfo = objectSpace.TypesInfo.FindTypeInfo(user2.GetType());
            typeInfo.FindMember("UserName").SetValue(user2, userName);
            user2.CallMethod("SetPassword",new[]{typeof(string)}, passWord);
            var roleCollection = (XPBaseCollection)typeInfo.FindMember("Roles").GetValue(user2);
            foreach (var role in roles) {
                roleCollection.BaseAdd(role);
            }
            return user2;
        }

        public static ISecurityRole GetAdminRole(this IObjectSpace objectSpace, string roleName) {
            var roleType = ((IRoleTypeProvider)SecuritySystem.Instance).RoleType;
            var administratorRole = (ISecurityRole)objectSpace.FindObject(roleType, new BinaryOperator("Name", roleName));
            if (administratorRole == null) {
                administratorRole = (ISecurityRole) objectSpace.CreateObject(roleType);
                var systemRoleBase = administratorRole as SecuritySystemRoleBase;
                if (systemRoleBase != null){
                    systemRoleBase.Name = roleName;
                    systemRoleBase.IsAdministrative = true;
                }
                else{
                    var permissionPolicyRole = ((IPermissionPolicyRole) administratorRole);
                    permissionPolicyRole.Name = roleName;
                    permissionPolicyRole.IsAdministrative = true;
                }
            }
            return administratorRole;
        }

        public static XpandRole GetAnonymousRole(this IObjectSpace objectSpace, string roleName, bool selfReadOnlyPermissions = true) {
            var anonymousRole = (XpandRole) objectSpace.GetRole(roleName);
            anonymousRole.Permissions.AddRange((IEnumerable<XpandPermissionData>) new[]{
                objectSpace.CreateModifierPermission<MyDetailsOperationPermissionData>(Modifier.Allow),
                objectSpace.CreateModifierPermission<AnonymousLoginOperationPermissionData>(Modifier.Allow)
            });
            return  anonymousRole;
        }


        static IModifier CreateModifierPermission(this IObjectSpace objectSpace, Modifier modifier,Type objectType){
            var operationPermissionData = (IModifier) objectSpace.CreateObject(objectType);
            operationPermissionData.Modifier = modifier;
            return operationPermissionData;
        }

        public static IModifier CreateModifierPermission<T>(this IObjectSpace objectSpace, Modifier modifier) where T : IModifier {
            return CreateModifierPermission(objectSpace, modifier, typeof(T));
        }

        public static ISecurityRole GetRole(this IObjectSpace objectSpace, string roleName,bool selfReadOnlyPermissions=true) {
            var roleType = ((IRoleTypeProvider)SecuritySystem.Instance).RoleType;
            var securityDemoRole = (ISecurityRole)objectSpace.FindObject(roleType, new BinaryOperator("Name", roleName));
            if (securityDemoRole == null) {
                securityDemoRole = (ISecurityRole)objectSpace.CreateObject(roleType);
                var systemRoleBase = securityDemoRole as SecuritySystemRoleBase;
                if (systemRoleBase != null){
                    systemRoleBase.Name = roleName;
                    if (selfReadOnlyPermissions) {
                        systemRoleBase.GrandObjectAccessRecursively();
                    }
                }
                else{
                    var permissionPolicyRole = ((IPermissionPolicyRole) securityDemoRole);
                    permissionPolicyRole.Name = roleName;
                }
            }
            return securityDemoRole;
        }

        [Obsolete("Use AddNewTypePermission<TObject>() instead (does same thing, only renamed)")]
        public static SecuritySystemTypePermissionObject CreateTypePermission<TObject>(this SecuritySystemRoleBase role, Action<SecuritySystemTypePermissionObject> action, bool defaultAllowValues = true) {
            return AddNewTypePermission<TObject>(role, action, defaultAllowValues);
        }

        [Obsolete("Use AddNewTypePermission<TObject>() instead (does same thing, only renamed)")]
        public static SecuritySystemTypePermissionObject CreateTypePermission<TObject>(this SecuritySystemRoleBase role){
            return AddNewTypePermission(role, typeof(TObject));
        }

        [Obsolete("Use AddNewTypePermission() instead (does same thing, only renamed)")]
        public static SecuritySystemTypePermissionObject CreateTypePermission(this SecuritySystemRoleBase role, Type targetType){
            return AddNewTypePermission(role, targetType);
        }

        [Obsolete("Use AddNewTypePermission() instead (does same thing, only renamed)")]
        public static SecuritySystemTypePermissionObject CreateTypePermission(this SecuritySystemRoleBase systemRole, Type targetType, Action<SecuritySystemTypePermissionObject> action,
                                                                                             bool defaultAllowValues = true){
            return AddNewTypePermission(systemRole, targetType, action, defaultAllowValues);
        }

        public static SecuritySystemTypePermissionObject AddNewTypePermission<TObject>(this SecuritySystemRoleBase role, Action<SecuritySystemTypePermissionObject> action, bool defaultAllowValues = true){
            return AddNewTypePermission(role, typeof(TObject), action, defaultAllowValues);
        }

        public static SecuritySystemTypePermissionObject AddNewTypePermission<TObject>(this SecuritySystemRoleBase role){
            return AddNewTypePermission(role, typeof(TObject));
        }

        public static SecuritySystemTypePermissionObject AddNewTypePermission(this ISecurityRole role, Type targetType){
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(role);
            var systemRole = role as SecuritySystemRoleBase;
            if (systemRole!=null){
                var permissionObject = objectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                permissionObject.TargetType = targetType;
                systemRole.TypePermissions.Add(permissionObject);
                return permissionObject;
            }
            return null;
        }

        public static SecuritySystemTypePermissionObject AddNewTypePermission(this SecuritySystemRoleBase role, Type targetType, Action<SecuritySystemTypePermissionObject> action,
                                                                                                     bool defaultAllowValues = true) {
            var permission = AddNewTypePermission(role, targetType);
            permission.AllowDelete = defaultAllowValues;
            permission.AllowNavigate = defaultAllowValues;
            permission.AllowRead = defaultAllowValues;
            permission.AllowWrite = defaultAllowValues;
            permission.AllowCreate = defaultAllowValues;
            action?.Invoke(permission);
            return permission;
        }

        [Obsolete("Use AddNewMemberPermission() instead (does same thing, only renamed)")]
        public static SecuritySystemMemberPermissionsObject CreateMemberPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemMemberPermissionsObject> action, bool defaultAllowValues = true) {
            return AddNewMemberPermission(securitySystemTypePermissionObject, action, defaultAllowValues);
        }

        public static SecuritySystemMemberPermissionsObject AddNewMemberPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemMemberPermissionsObject> action, bool defaultAllowValues = true)
        {
            IObjectSpace objectSpace = XPObjectSpace.FindObjectSpaceByObject(securitySystemTypePermissionObject);
            var permission = objectSpace.CreateObject<SecuritySystemMemberPermissionsObject>();
            permission.AllowRead = defaultAllowValues;
            permission.AllowWrite = defaultAllowValues;
            permission.EffectiveRead = defaultAllowValues;
            permission.EffectiveWrite = defaultAllowValues;
            securitySystemTypePermissionObject.MemberPermissions.Add(permission);
            action.Invoke(permission);
            return permission;
        }

        [Obsolete("Use AddNewObjectPermission() instead (does same thing, only renamed)")]
        public static SecuritySystemObjectPermissionsObject CreateObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, bool defaultAllowValues = true) {
            return AddNewObjectPermission(securitySystemTypePermissionObject, defaultAllowValues);   
        }

        public static SecuritySystemObjectPermissionsObject AddNewObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, bool defaultAllowValues = true) {
            return AddNewObjectPermission(securitySystemTypePermissionObject, null, defaultAllowValues);
        }

        [Obsolete("Use AddNewObjectPermission() instead (does same thing, only renamed)")]
        public static SecuritySystemObjectPermissionsObject CreateObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemObjectPermissionsObject> action, bool defaultAllowValues = true) {
            return AddNewObjectPermission(securitySystemTypePermissionObject, action, defaultAllowValues);
        }

        public static SecuritySystemObjectPermissionsObject AddNewObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemObjectPermissionsObject> action, bool defaultAllowValues = true) {
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(securitySystemTypePermissionObject);
            var permission = objectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
            permission.AllowDelete = defaultAllowValues;
            permission.AllowNavigate = defaultAllowValues;
            permission.AllowRead = defaultAllowValues;
            permission.AllowWrite = defaultAllowValues;
            permission.EffectiveDelete = defaultAllowValues;
            permission.EffectiveNavigate = defaultAllowValues;
            permission.EffectiveRead = defaultAllowValues;
            permission.EffectiveWrite = defaultAllowValues;
            securitySystemTypePermissionObject.ObjectPermissions.Add(permission);
            action?.Invoke(permission);
            return permission;
        }

        public static void CreatePermissionBehaviour(this ISecurityRole systemRole, Enum behaviourEnum, Action<ISecurityRole, ITypeInfo> action) {
            var typeInfos = XafTypesInfo.Instance.PersistentTypes.Where(info => {
                var permissionBehaviorAttribute = info.FindAttribute<PermissionBehaviorAttribute>();
                return permissionBehaviorAttribute != null && permissionBehaviorAttribute.Name.Equals(Enum.GetName(behaviourEnum.GetType(), behaviourEnum));
            });
            foreach (var typeInfo in typeInfos) {
                action.Invoke(systemRole, typeInfo);
            }
        }

        [Obsolete("Use AddNewFullPermissionAttributes() instead (does same thing, only renamed)")]
        public static void CreateFullPermissionAttributes(this SecuritySystemRoleBase systemRole, Action<SecuritySystemTypePermissionObject> action = null, bool defaultAllowValues = true) {
            AddNewFullPermissionAttributes(systemRole, action, defaultAllowValues);
        }

        public static void AddNewFullPermissionAttributes(this ISecurityRole securityRole, Action<SecuritySystemTypePermissionObject> action = null, bool defaultAllowValues = true) {
            var persistentTypes = XafTypesInfo.Instance.PersistentTypes.Where(info => info.FindAttribute<FullPermissionAttribute>() != null);
            foreach (var typeInfo in persistentTypes){
                var securitySystemRole = securityRole as SecuritySystemRoleBase;
                securitySystemRole?.AddNewTypePermission(typeInfo.Type, action, defaultAllowValues);
                var permissionPolicyRole = securityRole as IPermissionPolicyRole;
                permissionPolicyRole?.AddTypePermission(typeInfo.Type,SecurityOperations.FullAccess,defaultAllowValues?SecurityPermissionState.Allow : SecurityPermissionState.Deny);
            }
        }

        public static bool IsGranted(IPermission permission, bool isGrantedForNonExistent) {
            var securityComplex = (SecuritySystem.Instance as SecurityBase);
            if (securityComplex != null) {
                bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
                securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistent;
                bool granted = SecuritySystem.IsGranted(permission);
                securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
                return granted;
            }
            return SecuritySystem.IsGranted(permission);
        }

        public static List<IOperationPermission> GetPermissions(this ISecurityUserWithRoles securityUserWithRoles) {
            var permissions = new List<IOperationPermission>();
            foreach (ISecurityRole securityRole in securityUserWithRoles.Roles) {
                var operationPermissionProvider = (securityRole as IOperationPermissionProvider);
                if (operationPermissionProvider != null){
                    var operationPermissions = operationPermissionProvider.GetPermissions();
                    permissions.AddRange(operationPermissions);
                }
                if (securityRole is IPermissionPolicyRole){
                    var xpandRoleCustomPermissions = securityRole as IXpandRoleCustomPermissions;
                    if (xpandRoleCustomPermissions != null)
                        permissions.AddRange(xpandRoleCustomPermissions.GetCustomPermissions());
                    var supportHiddenNavigationItems = securityRole as ISupportHiddenNavigationItems;
                    if (supportHiddenNavigationItems != null)
                        permissions.AddRange(supportHiddenNavigationItems.GetHiddenNavigationItemPermissions());
                }
            }
            return permissions;
        }

        public static bool IsNewSecuritySystem(this IRoleTypeProvider security) {
            return security is SecurityStrategyComplex;
        }

        public static bool IsGranted(this IRole role, IPermission permission) {
            var permissionSet = new PermissionSet(PermissionState.None);
            role.Permissions.Each(perm => permissionSet.AddPermission(perm));
            var getPermission = permissionSet.GetPermission(typeof(ObjectAccessPermission));
            return getPermission != null && permission.IsSubsetOf(getPermission);
        }
    }
}
