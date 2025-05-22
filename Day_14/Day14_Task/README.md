# SOLID Principles

### S - Single Responsibility Principle
* Each class has only one responsibility
    * `UserRepository` handles data
    * `UserService` handles business logic.
    * `User` model class handles fields

### O - Open/Closed Principle
* I used interface `INotifier` that supports new the `Notifier` which can be extended without modification of existing code.
    * `EmailNotifier` and `SmsNotifier` are the current existing concrete class or we can say notifier classes.
    * If we want add any extension like `WhatsappNotifier` , we can add implement the interface to the class without affecting other codes.

### L - Liskov Substitution Principle
* UserService can use any INotifier without knowing its details — no broken behavior.
    * Because, `EmailNotifier` and `SmsNotifier`(subclasses) implemented the `INotifier`'s(base class) expected behaviour.
    * Without any correctness of behaviour like `throw notImplementedException` or some action which is not the expected behaviour of base class.

### I - Interface Segregation Principle
* There are two small interfaces - `INotifier`, `IOnlineNotifier`
    * `IOnlineNotifier`- This interface only for online based user not for normal users.
    * Instead of declaring the both method in `INotifier`, I segregated the it with interfaces.

### D - Dependency Inversion Principle
* UserService depends on the INotifier abstraction, not on concrete notifier implementations.
    * I passed the `INotifer` instance, So we don't have to change the Notifer concrete class in `Service class` each time.
    * We can simply pass the `Notifer` what we want, It can be `Email` or `Sms`.