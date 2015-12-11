#TableButtonExample

##Overview

This project serves as an example of handling and invoking tap events of UIButtons within custom UITableViewCell implementations within a Xamarin.iOS project.

In this project, a custom UITableViewCell implementation contains a UIButton that, when tapped, executes a function within the View Controller. The process is done by setting up a "stack" of events between the UIButton and the View Controller, and connecting them with event handlers.

Generally the connections work like so: View Controller <--> UITableViewSource <--> UITableViewCell <--> UIButton

This pattern is not specific to UIButtons, but rather it can be implemented for any event-based element added within custom UITableViewCell implementations.


##License & Authorship
 * This example project was created by William Thomas - [http://www.willseph.com/](http://www.willseph.com/)
 * Licensed under the WTFPL Public License v2 (WTFPL-2.0) - [https://www.tldrlegal.com/l/wtfpl](https://www.tldrlegal.com/l/wtfpl)
