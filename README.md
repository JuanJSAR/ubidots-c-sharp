# Ubidots C# API Client

The Ubidots C# API Client makes calls to the [Ubidots API](http://ubidots.com/docs/api/index.html).

## 1. How to use it

### 1.1 Manual Installation
Just open the solution in Visual Studio and build it. After that you will have the file _Ubidots.dll_ in your _/bin_ folder and you can add it as a reference to another project you are working on.

### 1.2 Using NuGet
You can use **NuGet** to install our library. Just search for _Ubidots_ and it will download our library and the required dependencies.

You can see more info here: https://www.nuget.org/packages/ubidots-csharp/1.0.0

## 2. Dependencies

Ubidots C# API Client uses the following libraries:

* [Newtonsoft Json.NET](http://www.newtonsoft.com/json), Popular high-performance JSON framework for .NET

To download the latest version _at the time of writing_ this library, you can follow the link below:

* [Newtonsoft Json.NET (version 6.0.8)](https://github.com/JamesNK/Newtonsoft.Json/releases/tag/6.0.8)

## 3. Using the library

### 3.1 Connecting to the API

Before you can start using all the stuff that Ubidots API provides, you should open an account and grab your API key. This API Key can be found in [your profile](https://app.ubidots.com/userdata/api/).

If you don't have an account yet, create one [here](http://app.ubidots.com/accounts/signup/)!

Once you have your API key, you can connect to the API by creating an ApiClient instance. After you have added Ubidots C# API Client as a reference in your solution (_see **1. How to use it**_) you must tell your program to use the classes available in the Ubidots namespace:

```csharp
using Ubidots;
```

Let's assume your API key is: _"WYp3qiKjP4AkymHZpVCh9zquim1Toh88twRD2"_, then your code will look like this:

```csharp
ApiClient Api = new ApiClient("WYp3qiKjP4AkymHZpVCh9zquim1Toh88twRD2");
```

Now you have an instance of the ApiClient class in the variable **Api** which can be used to make use of the API actions.

### 3.2 Accessing a DataSource

As you may know a DataSource represents a device or a virtual source.

You can access the information of a single DataSource with the method GetDataSource() which receives a string with your DataSource ID as a parameter. Let's asume we have a DataSource with an ID: _"0AkKNyF6VUiHAwP7obgC"_.

```csharp
DataSource ExampleDataSource = Api.GetDataSource("0AkKNyF6VUiHAwP7obgC");
```

If we want to bring a list of all the DataSources in our accoount we can call the method GetDataSources():

```csharp
DataSource[] ExampleDataSources = Api.GetDataSources();
```

### 3.3 Creating a DataSource

In the case where you want to create a new DataSource in your account, you just have to call the method CreateDataSource(), which receives a string containing the name of the DataSource.

```csharp
DataSource NewDataSource = Api.CreateDataSource("ExampleDataSource");
```

### 3.4 Accessing a Variable

A Variable contains different values over time, you can have multiple Variables in a DataSource.

If you want to access a variable using its ID, you can call the method GetVariable() which receives a string containing the ID of the Variable. Let's asume we have a Variable with the ID: _"hp7ikZQCTf3bjnjjcQo9"_.

```csharp
Variable ExampleVariable = Api.GetVariable("hp7ikZQCTf3bjnjjcQo9");
```

If you want to get the list of all variables there are two ways to do it:

1. By using the method GetVariables() of the ApiClient class, which will return a list with all the Variables in your account.
2. By using the method GetVariables() of the DataSource class, which will return a list with all the Variables of the DataSource.

```csharp
// Using ApiClient class
Variable[] ExampleVariables = Api.GetVariables();

// Using DataSource class
Variable[] ExampleVariablesDataSource = NewDataSource.GetVariables();
```

### 3.5 Creating a Variable

If you want to create a Variable, you can call the method CreateVariable() of the DataSource class, which receives a string containing the name of the new Variable.

```csharp
Variable NewVariable = NewDataSource.CreateVariable("NewExampleVariable");
```

### 3.6 Accessing the Values in a Variable

If you want to access to all the values of a Variable you can call the method GetValues() of the Variable class.

```csharp
Value[] ExampleValues = NewVariable.GetValues(); 
```

### 3.7 Saving a Value

If you want to save a new Value in your Variable you can call the method SaveValue() of the Variable class, that receives an integer or double containing the new Value.

```csharp
NewVariable.SaveValue(1337);
```

### 3.8 Saving multiple Values

If you want to save a bulk of Values in your Variable you can call the method SaveValues() of the Variable class, that receives an array of integers or doubles containing the new Values and an array longs containing the timestamps.

```csharp
int[] Values = new int[5] {1, 2, 3, 5, 8};
long[] Timestamps = new long[5] {1380558972614l, 1380558972915l, 
1380558973516l, 1380558973617l, 1380561122434l}

NewVariable.SaveValues(Values, Timestamps);
```

### 3.9 Using statistics endpoint

You can use the Ubidots statistics endpoint by calling the specific method:

* _getMean()_
* _getVariance()_
* _getMin()_
* _getMax()_
* _getCount()_
* _getSum()_

This methods are present in your Variable instance.

Usage example:

```csharp
Variable MySpecificVariable = Api.GetVariable("56799cf1231b28459f976417");
Double Mean = MySpecificVariable.getMean();
```
