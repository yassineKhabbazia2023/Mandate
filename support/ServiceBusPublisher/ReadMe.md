# Service Bus Publisher
Ce projet permet de publier des messages en masse sur le service bus.

## Configuration
La configuration se fait via le fichier `appsettings.json`, en valorisant le champ `ServiceBusConnectionString`.

```json
{
"ServiceBusConnectionString": "[MyServiceBusConnectionString]"
}
```

## Publication
Pour simplifier son utilisation sur le serveur de rebond, le projet est configuré pour être publié en un seul fichier.
La publication se fait via l'IDE ou la commande `dotnet publish -r win-x64`. 

## Utilisation
Copier l'ensemble des fichiers précédemment publiés, ainsi que le répertoire `MessagesToSend` contenant l'ensemble des
fichiers json des messages à publier et le fichier `appsettings.json` correctemement rempli.
Lancer `l'application ServiceBusPublisher.exe`.
