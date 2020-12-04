# Tanulókártya
## Jakabovics Zsombor - EA38MN

#### Szakdolgozatom céljaként fullstack webalkalmazást készítettem korszerű technológiák felhasználásával, amely segítséget nyújt a pedagógusok számára a tananyagfejlesztésben és ezzel a hatékonyabb munkavégzésben.

## Technológiák:
- ASP.NET Core 3.1
- Entity Framework Core
- Microsoft SQL Server
- Angular 8
- NodeJS v12.11.0
- Bootstrap 4
- DinkToPdf 1.0.8
- MailKit 2.0.3
- Moq 4.14.7
- xUnit 2.4.0
- Swagger

## Beüzemelés:
#### Az adatbázis létrehozása automatikusan történik, melyben a kezdő adatok létrejönnek a DbInitializer.cs fájl alapján.
#### A projekt Visual Studio segítségével futtatható legegyszerűbben, ahol is a projekt elindulása után az "npm install" parancs feltelepíti a kliens oldalhoz szükséges csomagokat.
#### Amennyiben az alkalmazás elindult, a "http://localhost:4200/" címen érhető el böngészőből.
#### Az API-k eléréséhez egy módszert kínál a Swagger UI, mely elérhető a "http://localhost:4200/swagger" oldalon.

## DinkToPdf dll letöltése:
#### A PDF generálásához szükséges letölteni a https://github.com/rdvojmoc/DinkToPdf/tree/master/v0.12.4 oldalról a megfelelő (32 vagy 64 bit) verziót és elhelyezni a gyökérkönyvtárban, ahol a Startup.cs fájl is található.

## Teszt felhasználók:
#### Az alkalmazásomban 9 különböző szerepkört vehetnek fel a felhasználók, melyből egyszerre 2 szerepkörrel rendelkezhet valaki. A szerepkörök között az adminisztrátor szabadon módosíthat.
#### A teszt felhasználók bejelentkezéshez szükséges adatai email és jelszó párosítással:
- admin@gmail.com - admin
- kartyakeszito@gmail.com - kartyakeszito
- koordinator@gmail.com - koordinator
- folektor@gmail.com - folektor
#### A felhasználók mellett egy kész próba kártyacsomagot vettem fel kész, aktivált állapotban.

## Üzemeltetés és éles környezet:
#### A pedagógusok által használt éles környezet elérhető a "https://tanulokartyak.azurewebsites.net/" címen.

## Tesztelés:
#### A FlashcardTests projekt tartalmazza a Unit teszteseteket, melyek Visual Studio alatt futtathatóak a Test Explorer segítségével.