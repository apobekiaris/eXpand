# About

_eXpandFramework_ is the first open source project based on the _DevExpress eXpressApp Framework (XAF)_. The _eXpressApp Framework_ ships with _DXperience Universal_ and can be evaluated free of charge (30 day trial) at: <https://www.devexpress.com/ClientCenter/Downloads.>

_eXpandFramework_ was started and is led by [Tolis Bekiaris](http://apobekiaris.blogspot.com/). _Tolis_ has made a huge contribution to _XAF_ over the years receiving great recognition from _DevExpress_ and the whole _XAF_ community. In _April 2011 Tolis_ was honoured to be hired as **Technical Evangelist for DevExpress frameworks**!

The _eXpandFramework_ is an extension of XAF and operates within the **Microsoft Public License (Ms-PL)**. Although _XAF_ is not an open source product it can be purchased directly from DevExpress.

The _eXpandFramework_ team have extended the capabilities of the _eXpressApp Framework_ to include **61 cutting-edge assemblies** containing tools and modules that target numerous business scenarios.

The main idea _behind eXpandFramework_ is to offer as many features as possible to developers/business users  through a declarative approach (configuring files rather than writing code). Please go through each of the following brief descriptions and find out how _eXpandFramework_ can help you accomplish your complex development tasks with easy declarative programming. Below you can see some descriptions and screenshots of our basic modules (screenshots taken from eXpand FeatureCenter application that comes along with the download). In the folder _Demos_ you can find a list of demos like _XVideoRental_, _SecurityDemo_, _MiddleTier_, _Workflow_ and _installation helper solutions for each module_.

## Modules

Examples of those modules include (in the two right columns you can see the supported platform):

Module Name | Description | ![Windows](http://www.expandframework.com/images/site/windows.jpg "Windows") | ![ASPNET](http://www.expandframework.com/images/site/aspnet.jpg "ASPNET")
------------|-------------|---------|-------
[ModelDifference](#model-difference) | Model managment | Y | Y
[Dashboard](#dashboard) |    Enables native XAF dashboard collaboration and integrates the Dashboard suite |    Y |    Y
WorldCreator |  Design runtime assemblies | Y | Y
Email |     Send emails using bussiness rules from application model without coding (see <http://goo.gl/Hkx6PK)> |    Y|Y
JobSheduler    | Acts as a wrapper for the powerfull Quartz.Net, providing a flexible UI for managing Jobs at runtime |    Y |    Y
WorkFlow |    Contains workflow related features (Scheduled workflows) |    Y |    Y
DBMapper |    Map 14 different types of databases at runtime into worldcreator persistent objects    | Y |    Y
IO |    Export & Import object graphs |    Y |    Y
ExcelImporter |    Imports Excel, csv files. |    Y |    Y
MapView |     Google Maps integration for XAF web apps. Blog posts. |     Y |    Y
FileAttachments |     Provides support for file system storage as per E965 |    Y |    Y
Scheduler |     Please explore the XVideoRental module found in Demos/XVideoRental folder (Blog posts) |    Y |    Y
Reports |     Please explore the XVideoRental module found in Demos/XVideoRental folder    | Y |    N
Chart |      Please explore the XVideoRental module found in Demos/XVideoRental folder     |Y |    N
PivotGrid |      Please explore the XVideoRental module found in Demos/XVideoRental folder |    Y |    N
HtmlPropertyEditor |      File upload and configuration through Application Model |    N |    Y
Import Wizard |    Universal module for importing excel files into any XAF application.     |Y|    N
Core|    Support multiple datastore , calculable properties at runtime ,dynamic model creation,control grid options, datacaching, web master detail, view inheritance etc.    |Y|    Y
WorkFlow|    Extends  XAF's workflow module to support schedule and on object changed workflows|    Y|    Y
AuditTrail    |Configures XAF Audit Trail module using the Application Modules. (see Declarative data auditing)|    Y|     Y
StateMachine|    Enhance XAF's statemachine module in order to control transitions using permissions    |Y|    Y
Logic|    Define conditional architecture    |Y|    Y
ModelArtifact|    Parameterize model artifacts (Controllers, Actions, Views)|    Y|    Y
AdditionalViewControlsProvider    |Decorate your views with custom controls|    Y    |Y
MasterDetail|    XtraGrid support for master-detail data presentation using the model.    Y|    N|
PivotChart|    Enhance analysis procedures / controls|    Y|    Y
Security|    Provides extension methods, authentication providers, login remember me, custom security objects|     Y|    Y|
MemberLevelSecurity|    Conditional security for object members.|    Y|    N
FilterDatastore|    Filter data at low level|    Y    |Y
Wizard|    Design wizard driven views|    Y    |N
ViewVariants|    Create views without the use of model editor|    Y|    Y
Validation|    More rules , permission validation, warning/info support, Action contexts etc|    Y|    Y
ConditionalObjectViews|    Allows the conditional navigation to your detailviews/listviews-->Merged with ModelArtifact    |Y|    Y
EasyTests|    Custom command and extensions for EasyTest see <http://apobekiaris.blogspot.gr/search/label/EasyTest>|    Y|    Y
TreelistView|    Enhance hierarchy controls, map XtraTreeList options to model|    Y|    Y
NCarousel|    Loads images asynchronously and displays them using a configurable carousel listeditor|    N|    Y
VSIX Package|    Enhance Model Editor, Explore Xaf Errors, Drop Database at design time, ProjectConverter invocation|

### Dashboard

Blogs:\
<http://apobekiaris.blogspot.gr/search/label/dashboard>

### Model Difference

Extends XAF by adding great new features for example:

* the ability to generate runtime members for your objects
* creating Application/Role/User models in the database
* storing your web cookies in the database
* handling of external application models
* combine end user modifications with application model
* support for multiple models at design time

<!-- <img src="/markdown/images/mdo1.png" alt="alt text" width="250"> -->
![""](/markdown/images/mdo1.png)
![](https://gyazo.com/eb5c5741b6a9a16c692170a41a49c858.png | width=100)aaa
<img src="images/site/firstpage/excelimporter.win.png" alt="excelimporter.win" width="613" height="419" /><br /><img src="images/site/firstpage/excelimporter.web.png" alt="excelimporter.web" width="617" height="570" />