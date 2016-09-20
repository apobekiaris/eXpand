﻿using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelClassSupressConfirmation : IModelNode {

        [Category(AttributeCategoryNameProvider.Xpand+ ".Modifications")]
        bool SupressConfirmation { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassSupressConfirmation), "ModelClass")]
    public interface IModelObjectViewSupressConfirmation : IModelClassSupressConfirmation {
    }
    public class SupressConfirmationController:ViewController<ObjectView>,IModelExtender{
        private ModificationsController _modificationsController;
        private ModificationsHandlingMode _modificationsHandlingMode;

        protected override void OnActivated(){
            base.OnActivated();
            if (((IModelObjectViewSupressConfirmation)View.Model).SupressConfirmation) {
                _modificationsController = Frame.GetController<ModificationsController>();
                _modificationsHandlingMode = _modificationsController.ModificationsHandlingMode;
                _modificationsController.ModificationsHandlingMode = (ModificationsHandlingMode) (-1);
                ObjectSpace.Committed += ObjectSpaceOnCommitted;
                if (View is DetailView && ObjectSpace.IsNewObject(View.CurrentObject)){
                    ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
                }
            }
        }


        public bool SupressConfirmation { get; set; }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_modificationsController != null)
                _modificationsController.ModificationsHandlingMode=_modificationsHandlingMode;
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            ObjectSpace.Committed-=ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs){
            _modificationsController.ModificationsHandlingMode = (ModificationsHandlingMode)(-1);
        }

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            _modificationsController.ModificationsHandlingMode = (ModificationsHandlingMode)(-1);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass,IModelClassSupressConfirmation>();
            extenders.Add<IModelObjectView,IModelObjectViewSupressConfirmation>();
        }
    }
}
