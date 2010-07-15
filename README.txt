Overview
========

At runtime Punchy minifies and combines javascript and css files into pre-configured bundles
for use by your web application. Most of the time you want to perform this step during the build
process, but occasionally it can be helpful to do it at runtime. Such as when you need to distribute
source code without minififaction tool dependencies.

Punchy is based on Pithy. Pithy does something very similar but uses a .NET port
of YUI Compressor. Punchy allows you to change compressors and includes YUI Compressor
for .NET and Microsoft Ajax Minifier.

Punchy requires that a folder within your web application be writable by the application.

Punchy uses file modification times to figure out if source files need to be re-minified.
Re-minified files are given cache-busting querystring parameters.

Change History
==============

2010-7-14 beta release
2010-7-13 broken do not use.
2010-7-12 alpha release