# Blend.Optimizely

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

## Features

### Blend Object Instance Cache

A convenience layer and abstraction around `ISynchronizedObjectInstanceCache`.

[Request More Info](https://github.com/Nhawdge/Blend.Episerver/issues/new?title=Blend%20Cache%20-%20Documenation%20Request&Body=)

### Enum Selection Factory

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

### Abstract Template Coordinator

A convenience class for making registering templates simple.

[Request More Info](https://github.com/Nhawdge/Blend.Episerver/issues/new?title=Abstract%20Template%20Coordinator%20-%20Documenation%20Request&Body=)

### Schedule Job Base

A convenient base class for scheduled jobs.

[Request More Info](https://github.com/Nhawdge/Blend.Episerver/issues/new?title=Scheduled%20Job%20Base%20-%20Documenation%20Request&Body=)


### Pagination Model

A simple model for calcuating pagination.

[Request More Info](https://github.com/Nhawdge/Blend.Episerver/issues/new?title=Pagination%20-%20Documenation%20Request&Body=)

