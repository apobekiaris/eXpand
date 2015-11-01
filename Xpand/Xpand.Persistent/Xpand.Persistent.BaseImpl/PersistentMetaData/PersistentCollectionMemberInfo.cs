using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [InterfaceRegistrator(typeof(IPersistentCollectionMemberInfo))]
    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "CollectionType,CollectionClassInfo")]
    [System.ComponentModel.DisplayName("Collection")]
    public class PersistentCollectionMemberInfo : PersistentMemberInfo, IPersistentCollectionMemberInfo {
        PersistentClassInfo _collectionClassInfo;
        Type _collectionType;
        string _collectionTypeFullName;
        RelationType _relationType;

        public PersistentCollectionMemberInfo(Session session) : base(session) {
        }

        public PersistentCollectionMemberInfo(PersistentAssociationAttribute persistentAssociationAttribute)
            : this(Session.DefaultSession, persistentAssociationAttribute) {
        }

        public PersistentCollectionMemberInfo(Session session,
                                              PersistentAssociationAttribute persistentAssociationAttribute)
            : base(session) {
            TypeAttributes.Add(persistentAssociationAttribute);
        }

        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof (TypeValueConverter))]
        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        public Type CollectionType {
            get { return _collectionType; }
            set {
                SetPropertyValue("CollectionType", ref _collectionType, value);
                if (!IsLoading && !IsSaving) {
                    _collectionTypeFullName = _collectionType != null ? _collectionType.FullName : null;
                    _collectionClassInfo = null;
                }
            }
        }

        #region IPersistentCollectionMemberInfo Members
        public RelationType RelationType {
            get { return _relationType; }
            set { SetPropertyValue("RelationType", ref _relationType, value); }
        }

        [Browsable(false)]
        public string CollectionTypeFullName {
            get { return _collectionTypeFullName; }
            set {
                SetPropertyValue("CollectionTypeFullName", ref _collectionTypeFullName, value);
            }
        }

        void IPersistentCollectionMemberInfo.SetCollectionTypeFullName(string s) {
            CollectionTypeFullName = s;
            if (_collectionType == null && _collectionClassInfo == null && s != null) {
                IPersistentClassInfo classInfo = PersistentClassInfoQuery.Find(Session, s);
                if (classInfo != null)
                    _collectionClassInfo = (PersistentClassInfo) classInfo;
                else
                    _collectionType = ReflectionHelper.GetType(s.Substring(s.LastIndexOf(".") + 1));
            }
        }
        public PersistentClassInfo CollectionClassInfo
        {
            get { return _collectionClassInfo; }
            set
            {
                SetPropertyValue("CollectionClassInfo", ref _collectionClassInfo, value);
                if (!IsLoading && !IsSaving)
                {
                    _collectionTypeFullName = _collectionClassInfo != null
                                                  ? _collectionClassInfo.PersistentAssemblyInfo.Name + "." +
                                                    _collectionClassInfo.Name
                                                  : null;
                    _collectionType = null;
                }
            }
        }

        IPersistentClassInfo IPersistentCollectionMemberInfo.CollectionClassInfo {
            get { return CollectionClassInfo; }
            set { CollectionClassInfo=value as PersistentClassInfo; }
        }
        #endregion
    }
}