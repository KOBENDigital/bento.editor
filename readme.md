<img src="./docs/img/bento.svg" width="250">

# Bento Editor for Umbraco 8

The Bento Editor is a content block editor for Umbraco 8 that takes advantage of the infinite editing features of Umbraco 8.

The aim of Bento is to simplify the experience of creating creative layouts in Umbraco without compromising brand and design standards.

It is heavily inspired by the best parts of the original Umbraco Grid Editor with the DTGE plugin, but with the simplicity of editors such as Stacked Content.

License: MIT

## :bento: Building and running the source

After forking and cloning the repo, open the VS solution and run the Bento.Website project.  It uses the embeded SQLCE database. This will enable you to play with some test content and learn how some of the concepts of bento work.

The Umbraco login details:

username: admin@admin.com
password: bentobentobento


## :bento: Install

Umbraco must be installed first.

Bento can be installed either via Nuget or by installing the Umbraco Package files from our.umbraco.com.  Bento is made up of two packages, ```Bento.Core``` which contains the services helpers and controllers and ```Bento.Editor``` which contains the backoffice editor experience.

>```Install-Package Bento.Editor```

If you are planning on splitting your project up you may install the ```Bento.Core``` on its own.

>```Install-Package Bento.Core```

Once installed you should be able to create a new DataType of either Bento Item or Bento Stack.

## TL DR: To get started you are going to need to:

1. Create a document type that will be used as a composition to indentify Bento Items
2. Create a  document type that will be used for type folders in a reusable library
3. Create a  document type that will be used for a reusable library\
4. Setup your Datatypes for Bento Item Bento Stack.
5. Create a set of document types to represent your various reusable content block Items and one use embedable Elements.  These need to inherit your Bento Item Composition.
6. Build awesome websites.

## :bento: Document Type Setup

Bento has been designed to be configured via document types and datatype prevalue settings. There are no complex configuration files.

### Bento Item Composition
This document type can be called anything you like.  It will be used as a composition on all Bento content items.  It is a good place to store any common properties that you might want on all of your Bento items such as a flag to hide a block from render.

### Bento Item Type Folder
Create a document type that has no template and has no properties.  This will be used as folder to contain reusable blocks.  You cna return here later to set any child item permissions you want later if you want to set restrictions if you are going to give editors access.

### Bento Library
Create document type that will be used as folder that will contain your Bento Item Type folders.  It can be created without a template and you will need permissions to create your Bento Item Type Folder below.

Create a library folder somewhere in your tree where you want to manage your reusable blocks.

### Bento Content Items and Elements

> TIP: Do the following doctype setup after you have configured the basics of the Datatypes in the next section of this documenation.  This will allow Bento to automatically create partial views for your Items and Elements.

What is the difference between a Content Item and an Element?  Content Items are published pieces of content that can be reused and are stored in the Bento Library.  Elements are items that are embedded directly into your Bento Stack or Bento Item.

Bento Items and Elements are just standard document types that have not standard Umbraco Tempaltes.  Make sure that when you create your Bento Content Items and Elements to NOT creat them with templates. Bento has its own partial views to represent items for frontend rendering and back office rendering.  These will be explained later.

Make sure that when creating both Content Items and Elements that you inherit the Bento Item Composition as this is a special bit of glue that allows Bento to identify its parts and to help automate certain creation tasks such as automatic partial view creation.

For Elements it is vitally important that you also set the permissions for the Elements to ```Is an element type``` under the ```Permissions``` tab.

>TIP: If you are planning on creating both a reusable Content Item and an Element, it is sometimes better to create a composition that both item and the element share that contains the properties.  This enables simplification in your code as both will share the common composition interface.\
\
For example we could create a Promo Item and a Promo Element that both inherit from Promo Comp.  Promo Comp would be where we setup all the properties that both the Item and Element require.\
\
This promotes reused and both views can share the common ```IPromoComp``` as the Model.

## :bento: Datatype Setup
Bento comes with two Umbraco Data Types, Bento Item and Bento Stack. Bento item is a simple single block editor.  This is useful for creating and managing items such as a carousel or header.

The Bento Stack is why you are really here. The Bento Stack the editor that allows you to create stacked blocks.  Each block can have its own layout that contains areas.  Layouts can be as simple as a single cell, or as complex as a mosaic oddly arranged tiles.  We achieve this in the back office using [CSS Grid Layouts](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Grid_Layout).

### Bento Item
Bento Item setup is straight forward.  Create a new DataType, either in the Data Type section or during Document Type creation.

1. Set each of the setting matching the document types you setup earlier to each of the settings.  
2. Set the allowed Content Types that can be created in the library and used in the block
3. Set the allowed Element Types that can be embbeded in the this block

> TIP: Its important that when you creating your Items and Elements that you make it easy to identify them.  A great way to do this is using folder in the Document Type section or by using an easy to follow naming convention. Remember Elements cannot be used as reusable Ttems and Items cannot be used as embedded Elements.

You should now be able to add your Bento Item type to a Document Type and create a Bento Item.

### Bento Stack
The Bento Stack is similar to the Bento Item setup except for a key difference that it requires Layouts. To setup a new stack either create a new Data Type in the Data Type section or during Document Type creation.

1. Set each of the settings to match the document types you created earlier.  Libary Type Folder, Library Folder and the Bento Item Composition.
2. Create a new Layout by clicking ```Add```
3. Fill out the layout setup. The panel is fairly self explanitory and provides a preview as you create your layout.  Layouts are based on [CSS Grid Layouts](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Grid_Layout) so it is helpful to have a small amount of knowledge but not vital.
4. Layouts can have Layout Settings.  Again these are just simple Document Types that contain properties that allow you to control a Layouts behavior. These are usful when you want to control things like Layout Margins, background colours and images etc.
5. Initially Layouts are not sortable and allow you to set permissions on each of the Layout Areas as to what Content Items and Elements they can contain. If you set that a Layout is sortable, you set permissable types at the parent Layout level rather than a the Area level.
6. Save your Bento Stack Data Type

You should now be able to add your Bento Stack Data Type to a Document Type.

>The Bento Stack is sortable vertically (Layouts can be shuffled up and down) by dragging the layouts in the stack, and when you allow sorting within a layout, you can rearrange the content of a layout by dragging items within the layout by its title.

> NOTE: you cannot drag items from one layout to another.

## :bento: Templates / Views
Bento Layouts, Items and Elements are all CSHTML views that live in the ```/views/partials/bento``` folder.


### Rendering the Bento Stack
Bento Stack properties are of type ```IEnumerable<Bento.Core.Models.StackItem>```. Rending a Bento Stack property can be done as follows:

```
@foreach(Bento.Core.Models.StackItem layout in Model.MyBentoStackProperty){
  <section>
      @Html.Partial($"~/Views/Partials/Bento/layouts/{item.Alias}.cshtml", item)
  </section>
}
```
### Layouts
Bento Layouts are created in the ```/views/partials/bento/layouts``` folder.  The naming convention for a layout is ```yourLayoutAlias.cshtml``` where the Layout Alias is the alias you gave the layout when setting up your Bento Stack Layouts for this editor.

Although you setup a layout in the back office using CSS Grid, the front end output is entirely up to you.  You can either continue ot use CSS Grid, or you could go with any other framework you wish eg. TailWindCSS or BootStrap.

Rendering a layout requires looping through the areas of a layout.  The ```StackItem``` class contains a collection fo Areas.  You can render out your areas in your layout tempalte like this:

```
@inherits Umbraco.Web.Mvc.UmbracoViewPage<Bento.Core.Models.StackItem>
@foreach (var area in Model.Areas.Where(x => x.Content != null))
  {

  <div class="@Model.Alias-@area.Alias">
    @Html.Partial($"~/Views/Partials/Bento/{area.Content.ContentType.Alias}.cshtml", area.Content)
  </div>
}
```
### Items and Elements
Item and Element views can be found in the folder ```/views/partials/bento``` as long as your document typese inherit the Bento Item Composition Document Type your item and element views should be automatically created for you.

There are two views per Item / Element. A front end view and a Back Office view that is identifiable by its ```BackOffice``` filename suffix. For example.

Front end view
> ```RichTextElement.cshtml```

Back Office View
> ```RichTextElementBackOffice.cshtml```

The generated views look like this

```
@inherits Umbraco.Web.Mvc.UmbracoViewPage<IPublishedElement>
<p>View for Bento doctype 'Rich Text Element' (alias: richTextElement)</p>
```
 and inherit the generic IPublished content model but you can easily change this out for the strong typed Models Builder Model generated by Umbraco. Eg
 ```
@inherits Umbraco.Web.Mvc.UmbracoViewPage<ContentModels.RichTextElement>
@using ContentModels = Umbraco.Web.PublishedModels;
 ```

## :bento: Experimental Features 
It is possible to render some styling in the back office.  This can be enabled via the ```Use back office CSS``` switch on the Bento Stack editor and supplying a path to a stylesheet in the ```CSS File Path``` field.

> TIP: If you do want to use this its important that your CSS is very specific and it it helps to contain all of your CSS in a master class that wraps all yoru custom classes.  This will help ensure your CSS does not spill over and start effecting Umbraco back office styles.

An example of the using the back office styler can be found in the source repo in the Web Project.
