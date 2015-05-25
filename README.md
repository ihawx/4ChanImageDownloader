###                                  Unoffical 4Chan Image Downloader
                             ****THIS IS NOT AN OFFICIAL 4CHAN APPLICATION****
                             
#####About
This simple program can search through all the boards on 4chan, look for a given key words through catalog
and in case it finds them, download all the images in a thread which contained the keyword.
Because the program only searches through the catalogs, it will only identify thread if the key word is in 
OP, subject of the thread or three last replies.

You can set the interval in between searches to whatever you want, but there's no need to search every second
as most boards are too slow anyway for you to miss out on a picture you want in case that someone replies with
a searched keyword.

You can also specify which words you want to ignore. In case that you choose to do so, every thread which
contains both the searched keyword and a word set as ignored will be ignored and won't be searched.

It also downloads the text from a found thread, but the final formating is far from perfect and still needs a lot work. Any suggestions about what to do with it are welcome.

To learn more about the 4Chan API visit this page: https://github.com/4chan/4chan-API

#### Screenshot

![Screenshot](http://i.imgur.com/sv8be9X.png)

#### Newtonsoft Json.NET library
This program uses the Newtonsoft Json.NET library which can be found here: http://www.newtonsoft.com/json

It uses the MIT License which is contained below:

The MIT License (MIT)
Copyright (c) 2007 James Newton-King
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
documentation files (the "Software"), to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

####About author
This program was created by iHawX (github: https://github.com/ihawx) in 2015. Use it, modify it, share it at your own will.
