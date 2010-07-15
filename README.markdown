Overview
========

Punchy is a .NET library for web applications that minifies and combines javascript and css files into pre-configured bundles at runtime. Most of the time you want to perform this step during the build process, but occasionally it can be helpful to do it at runtime. Such as when you have user-editable css, cannot distribute minification tools, or otherwise modify js/css without building. 

Punchy is based on [Pithy][1]. Pithy does something very similar but uses a .NET port of YUI Compressor exclusively. Punchy allows you to configure custom toolchains and includes tools for Less Css, YUI Compressor for .NET, and the vexingly named Microsoft Ajax Minifier.

Punchy requires that a folder within your web application be writable by the application. It has not been tested on Azure.

Punchy uses file modification times to figure out if source files need to be re-minified. Re-minified files are given cache-busting querystring parameters.

Change History
==============

2010-7-14 beta release

2010-7-12 alpha release

[1]: http://github.com/clearwavebuild/Pithy