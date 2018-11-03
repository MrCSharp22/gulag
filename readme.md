## RafaelWare IoC Container (Gulag)
A simple IoC container built for .Net Standard.

## Description
This project was created so I can learn and understand IoC and how the IoC containers work. It evolved a bit to the point that now I am using it in my own Apps.

This project is published on Github hoping that others can use it to learn more about IoC and how containers work.

Please keep in mind that this is not a fully featured IoC and was never intended to be.

## Tech/framework used
.Net Standard 1.0

## Code Example

``` 

//Register your services
IoC.Current.Register<IAmAnInterface>().Use<InterfaceImplementation>();
...

//Resolve the service and use it
IoC.Current.Resolve<IAmAnInterface>().DoSomething();

```

## Installation
//TODO nuget package will be provided later

## Contribute

If there is a feature you want to add to this, please fork the repo, build the feature and then create a pull request. I will review and merge it as soon as possible.

## License

GNU GPL © [Rafael]()