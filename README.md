# FlowProtocol 2

<div align="center"><img src="FlowProtocol2\wwwroot\Images\FlowProtocol2Logo40.jpg" alt="FlowProtocol 2 Logo" title="FlowProtocol 2" /></div>

## Zusammenfassung
*FlowProtocol 2* ist eine kleine überschaubare Open Source Anwendung für das Intranet, die einfache Skripte im Browser ausführen kann.
Die Skripte können dabei in einem normalen Texteditor wie Notepad++ erstellt, und in einer gegliederten Verzeichnisstruktur verwaltet werden. Die Auswahl erfolgt dann über die Oberfläche von *FlowProtocol 2*, wobei sich ein Skript auch direkt über eine URL aufrufen, und so z.B. als Lesezeichen im Browser oder als Link in einem Wiki bereitstellen lässt.
Die FlowScript-Sprache bietet die Möglichkeit, Auswahlfragen und Texteingaben an den Anwender zu richten und diese zu verarbeiten. Das Ergebnis besteht am Ende aus einem Dokument, das im Browser angezeigt wird und vom Anwender verwendet werden kann.

Die Anwendungsgebiete sind vielfältig. Die Anfangsidee für die erste Version bestand darin, Checklisten durch interaktive Fragestellungen speziell auf einen einzelnen Anwendungskontext hin auszurichten, und so genau die Prüfpunkte aufzulisten, die für den jeweils vorliegenden Fall wirklich relevant sind. Im Gegenzug konnten diese dann detaillierter ausfallen und zu einem besseren Ergebnis führen.
Durch den ausgiebigen Einsatz bei der eigenen Arbeit als Softwareentwickler kamen permanent weitere Anwendungsfälle und auch neue Funktionen hinzu, so dass inzwischen die Userstories vieler Standardentwicklungen durch Skripte erzeugt werden, und die dazugehörenden Entwicklungen auf Basis skriptgenerierter Anleitungen entstehen, wobei große Teile vom Programmcode direkt aus dem Anleitungsdokument übernommen werden können. Hinzu kommen zahlreiche kleinere Routineaufgaben, für die sich aus wenigen Eingaben und Optionen hilfreicher Batch- oder SQL-Code erzeugen lässt.

*FlowProtocol 2* und seine Skriptsprache sind bewusst einfach gehalten und erinnern an Basic, um sowohl die Schwelle für die Anwendung, als auch die für die Skripterstellung sehr niedrig zu setzen. Der besondere Nutzen der Anwendung besteht darin, dass die Erweiterung eines Skriptes unmittelbar im Bedarfsfall erfolgen kann, und in vielen Fällen weniger als 5 Minuten braucht. Änderungen im Skript lassen sich durch Aktualisieren der Browserseite direkt berücksichtigen, ohne dass man das Skript neu starten muss.
Diese Niederschwelligkeit macht es möglich, dass neu erkannte Fälle und Anhängigkeiten direkt durch die Anwender selbst kontinuierlich in die Skripte integriert werden, und so für alle nachfolgenden Anwender Nutzen bringen.

Diese Einfachheit spiegelt sich auch auf technischer Ebene wieder. Als einzige serverseitige Voraussetzung wird der Lesezugriff auf das Skriptverzeichnis benötigt. Es ist weder Schreibzugriff notwendig, noch eine Datenbank, es gibt keine Benutzerverwaltung und es werden auch keine APIs anderer Anwendungen angesprochen. Außer der Ergebnisdokumentseite werden keine Daten erzeugt. Diese kann Links enthalten, die auch ein Zusammenspiel mit anderen Browser-Anwendungen ermöglichen, und es gibt die Möglichkeit, Code-Passagen über eine Schaltfläche in die Zwischenablage zu kopieren.

*FlowProtocol 2* ist eine umfangreiche Neuentwicklung von *FlowProtocol*, das durch zahlreiche Erweiterungen und nachträgliche Ergänzungen immer wieder an Grenzen gekommen ist, insbesonderen aufgrund der gewachsenen Architektur. In *FlowProtocol 2* konnten die zahlreichen dabei gewonnenen Erfahrungen und Ideen von Anfang an berücksichtigt und konsequent umgesetzt werden, was sich auch in einer Überarbeitung der Sprache niedergeschlagen hat. Die direkte Weiterverwendung bestehender *FlowProtocol*-Vorlagen als *FlowProtocol 2*-Skripte ist daher nicht möglich, dennoch sind viele Befehle identisch und die Anpassung sollte meist ohne viel Aufwand möglich sein.

## Konfiguration
Die Konfiguration der Anwendung besteht primär darin, ein Verzeichnis für die Skripte einzurichten und den Pfad darauf in die Eigenschaft "ScriptPath" in der Datei appsettings.json einzutragen.


Zusätzlich besteht die Möglichkeit, über die Eigenschaft "LinkWhitelist" in der Datei appsettings.json eine Liste von Domänen und Adressen einzutragen, die als vertrauenswürdig eingestuft werden. Befinden sich Einträge in dieser Liste, so werden Links in den Skripten nur dann allein über den Anzeigetext dargestellt, wenn einer dieser Einträge mit dem Anfang der URL übereinstimmt. Andernfalls wird die URL dem Anzeigetext in Klammern nachgestellt ist so für den Anwender unmittelbar erkennbar.

Das zum Projekt gehörende Verzeichnis Scripts\FP2-Tutorial enthält Skripte für alle zur Sprache gehörenden Befehle, in denen diese an einem kleinen Beispiel kurz erklärt werden. Gerade zum Einlernen in die Syntax empfiehlt es sich, das Tutorial zu den eigenen Skripten hinzuzufügen.
