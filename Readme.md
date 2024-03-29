﻿# Blend.Optimizely

A collection of utilities and classes common to Blend Optimizely CMS 12 sites.

## License

See License File

## Goals

This is a collection of helpers and utility classes used in our Optimizely sites at [Blend Interactive](https://blendinteractive.com).

## How to contribute

1. Check for open issues or open a fresh issue to start a discussion around a feature idea or a bug.
1. Fork the repository on Github to start making your changes.
1. Write a test which shows that the bug was fixed or that the feature works as expected.
1. Send a pull request and bug the maintainer until it gets merged and published.
1. Make sure to add yourself to CONTRIBUTORS.txt.

# Popular Features

## EnumSelectionFactory

Usage:

```
[SelectOne(SelectionFactoryType = typeof(EnumSelectionFactory<ImageSizes>))]
public virtual ImageSizes ImageSize { get; set; }
// ...
public enum ImageSizes {
    Narrow,
    [Description("Standard Width")]
    StandardWidth,
    Wide,
}
```

## Has Value Extensions

A collection of extension methods to check if a property is not null, and has a value. Checks vary depending on what the object is, for example, `HasValue` on a list will check that the list is not null and contains at least one element.

Usage:

```
var alertsContentArea = StartPage.AlertsContentArea;
if (alertsContentArea.HasValue()) {
    <div>Alert</div>
}
```

Also includes a `Coalesce` method that uses `HasValue` to coalesce empty values to a default value.

Usage:

```
var pageTitle = currentContent.PageTitle.Coalesce(currentContent.Name);
```


### Link Resolver Service

The `LinkResolverService` gives a unified way of resolving references to content in Optimizely. It includes support for resolving `ContentReference`, `IContent`, `LinkItem`, and `Url` references. The resolved links can optionally follow internal shortcuts and include the full domain and scheme (for canonical links, for example). Links are resolved as `ResolvedLink` objects, which include the final resolved URL and the target attribute value to be used in anchor tags.

There is also a set of `ResolveUrl` extension methods that only return the URL for convenience. These extension methods replace the `GetFriendlyUrl` methods in this package which have been mostly obsoleted.

Usage:

```
// Using the service
var page = ContentLoader.Get<StandardPage>(10);
var resolved = ServiceLocator.Current.GetInstance<LinkResolverService>().ResolveIContent(page, LinkOptions.None);
var url = resolved is not null ? resolved.Href : "ERROR";

// Using the extension methods
var reference = new ContentReference(5);
string canonicalUrl = reference.ResolveUrl(LinkOptions.IncludeDomain);
```

### Get<T>

Simple extension to get the content T of a ContentReference.

Usage:
```
var reference = new ContentReference(5);
var pageFive = reference.Get<PageData>();
```

### GetChildren

Utility to get the children of a page.

Usage:
```
var myChildren = CurrentPage.GetChildren<PageData>();

<ul>
    @foreach (var child in myChildren) {
        <li>@child</li>
    }
</ul>
```

## Content Event Handler

The package automatically adds content events for the following interfaces. The method specified in the Interface is called when that event occurs.
This is a nice clean way to add Content events directly to the content class.

```
ICreatedContentHandler
ICreatingContentHandler
IDeletedContentHandler
IDeletingContentHandler
IPublishedContentHandler
IPublishingContentHandler
ISavedContentHandler
ISavingContentHandler
IMovingContentHandler
IMovedContentHandler
```


## Enum Selection Factory

*Usage*:
```
public enum DisplaySettings
{
    Narrow,	
    FullWidth,
}
//.....
[SelectOne(SelectionFactoryType = typeof(EnumSelectionFactory<DisplaySettings>))]
```


## Html Extensions 

### PropertyTemplateFor

A wrapper for PropertyFor that allows you to change out the tags used when rendering. Useful for Xhtmlstrings that are used for Headings.

### Get String

A tiny method to convert IHtmlContent into a string. This utility method is to account for a .Net 5 change to IHtmlString.

## Enum Localization

This is used by the Enum Selection Factory. This allows Enums to be localized by a `[Description]` attribute.

## ConvertTextAreaToHtml

Takes a multiline string of plain text and converts it to HTML. This automatically encodes HTML entities and converts newlines to `<br />` tags. This method should handle Windows, Mac, and Unix style breaks.

Usage:

```
var example = @"This is a <test>.
It should work.";

var output = example.ConvertTextAreaToHtml();
/* Output should be:
This is a &lt;test&gt;.<br />
It should work.
```

## Enumerable Extensions

### Chunk 

Splits an enumerable into chunks. For example a list of 7 in chunk sizes of 2, would return 3 groups of 2, and 1 group of 1

### Split 

Splits an enumerable into a set number of groups, group size is automatically determined.

### As Content

Gets the content for a `IList<ContentReference>` or `IEnumerable<ContentReference>` and returns an `IEnumerable<T>` 

## Content Extensions

### FindFirstAncestorOfType and FindLasttAncestorOfType

Two utility methods to do exactly what their name says. 

## Abstract Template Coordinator

A convenience class for making registering templates simple.

[Request More Info](https://github.com/BlendInteractive/Blend.Optimizely/issues/new?title=Abstract%20Template%20Coordinator%20-%20Documentation%20Request&Body=)

## Schedule Job Base

A convenient base class for scheduled jobs. Inherit your scheduled jobs from `BlendJobBase` to get started.
This Scheduled Job Starter adds a stop watch, an easy record tracker, and report to keep all your output clean.
Also included are numerous handy methods for getting all pages, and content publishing only if a change has occured.

[Request More Info](https://github.com/BlendInteractive/Blend.Optimizely/issues/new?title=Scheduled%20Job%20Base%20-%20Documentation%20Request&Body=)


## Blend Object Instance Cache

A convenience layer and abstraction around `ISynchronizedObjectInstanceCache`.

[Request More Info](https://github.com/BlendInteractive/Blend.Optimizely/issues/new?title=Blend%20Cache%20-%20Documentation%20Request&Body=)

## Pagination Model

A simple model for calcuating pagination.

[Request More Info](https://github.com/BlendInteractive/Blend.Optimizely/issues/new?title=Pagination%20-%20Documentation%20Request&Body=)

## Result class

The `Result<TValue, TError>` class is a way to represent results from an API call, handling both successful values and errors. It includes a number of methods to ensure successes and failures are handled. This is essentially an Either monad.

Usage:

```
// As an API result
public Result<string, UserError> GetUsername(int userId)
{
    // Using implicit conversion to convert `string` and `UserError` results to `Result<string, UserError>`
    if (!UserService.IsAuthenticated)
        return GetUsernameError.Unauthenticated;
    if (!UserService.IsCurrentUserAdmin)
        return GetUsernameError.AccessDenied;
    var user = UserService.GetUser(userId);
    if (user is null)
        return GetUsernameError.UserDoesNotExist;
    return user.Username;
}

// Note: The implicit conversion doesn't work for interfaces, so you can also use the `Result` static methods.
public Result<ISearchResult, SearchError> ExecuteSearch(Query query) {
    if (Search.IsDown)
        return SearchError.SearchIsDownAgain; // Implicit conversion
    
    ISearchResult result = Search.ExecuteQuery(query);
    return Result.Success<ISearchResult, SearchError>(result); // Using the static method to construct the Result.
}

// There are a few ways to consume a result:

// You can use the record as-is.
var result = searchService.ExecuteQuery(query);
if (!result.IsSuccess)
    return View("~/Search/Error.cshtml", result.Error);
else
    return View("~/Search/Results.cshtml", result.Value);


// You can destructure the record for quick access to values
var (results, error, isSuccess) = searchService.ExecuteQuery(query);
if (!isSuccess)
    return View("~/Search/Error.cshtml", error);
else
    return View("~/Search/Results.cshtml", results);

// If you're using nullable reference types, to avoid null check warnings, you can use the `HasSuccessValue` and `HasError` methods.
var result = searchService.ExecuteQuery(query);
if (result.HasError(out var error))
    return View("~/Search/Error.cshtml", error); // The compiler knows error is not null here.
else if (result.HasSuccessValue(out var results))
    return View("~/Search/Results.cshtml", results); // The compiler knows results it not null here.
else
    throw new NotImplementedException("Something was null that shouldn't have been!");

// If you want to force both success and error paths to be handled at compile time, you can use the `Switch` and `Match` extension methods, where `Switch` executes the correct path expecting no returned values, and `Match` executes the correct path but expects a returned value.

return searchService.ExecuteQuery(query).Match(
    (error) => View("~/Search/Error.cshtml", error),
    (results) => View("~/Search/Results.cshtml", results)
);
```

Note that both `Switch` and `Match` expect that for successful values, `Value` is not null and for non-successful values, `Error` is not null. Otherwise a `NotImplementedException` will be thrown. This is to avoid having to do null checks in the `onSuccess` and `onError` delegates.

