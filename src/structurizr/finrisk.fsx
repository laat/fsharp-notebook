#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: dotenv.net, 3.0.0"
#r "nuget: Structurizr.Core, 1.0.0"
#r "nuget: Structurizr.Client, 1.0.0"

open dotenv.net
open Structurizr
open Structurizr.Api

let AlertTag = "Alert"

let workspace =
    new Workspace(
        "Financial Risk System",
        "This is a simple (incomplete) example C4 model based upon the financial risk system architecture kata, which can be found at http://bit.ly/sa4d-risksystem"
    )

let model = workspace.Model

let financialRiskSystem =
    model.AddSoftwareSystem("Financial Risk System", "Calculates the bank's exposure to risk for product X.")

let businessUser =
    model.AddPerson("Business User", "A regular business user.")

businessUser.Uses(financialRiskSystem, "Views reports using")

let configurationUser =
    model.AddPerson(
        "Configuration User",
        "A regular business user who can also configure the parameters used in the risk calculations."
    )

configurationUser.Uses(financialRiskSystem, "Configures parameters using")

let tradeDataSystem =
    model.AddSoftwareSystem("Trade Data System", "The system of record for trades of type X.")

financialRiskSystem.Uses(tradeDataSystem, "Gets trade data from")

let referenceDataSystem =
    model.AddSoftwareSystem(
        "Reference Data System",
        "Manages reference data for all counterparties the bank interacts with."
    )

financialRiskSystem.Uses(referenceDataSystem, "Gets counterparty data from")

let referenceDataSystemV2 =
    model.AddSoftwareSystem(
        "Reference Data System v2.0",
        "Manages reference data for all counterparties the bank interacts with."
    )

referenceDataSystemV2.AddTags("Future State")

financialRiskSystem
    .Uses(referenceDataSystemV2, "Gets counterparty data from")
    .AddTags("Future State")

let emailSystem =
    model.AddSoftwareSystem("E-mail system", "The bank's Microsoft Exchange system.")

financialRiskSystem.Uses(emailSystem, "Sends a notification that a report is ready to")

emailSystem.Delivers(
    businessUser,
    "Sends a notification that a report is ready to",
    "E-mail message",
    InteractionStyle.Asynchronous
)

let centralMonitoringService =
    model.AddSoftwareSystem("Central Monitoring Service", "The bank's central monitoring and alerting dashboard.")

financialRiskSystem
    .Uses(centralMonitoringService, "Sends critical failure alerts to", "SNMP", InteractionStyle.Asynchronous)
    .AddTags(AlertTag)

let activeDirectory =
    model.AddSoftwareSystem("Active Directory", "The bank's authentication and authorisation system.")

financialRiskSystem.Uses(activeDirectory, "Uses for user authentication and authorisation")

let views = workspace.Views

let contextView =
    views.CreateSystemContextView(
        financialRiskSystem,
        "Context",
        "An example System Context diagram for the Financial Risk System architecture kata."
    )

contextView.AddAllSoftwareSystems()
contextView.AddAllPeople()

let styles = views.Configuration.Styles
financialRiskSystem.AddTags("Risk System")

styles.Add(new ElementStyle(Tags.Element, Color = "#ffffff", FontSize = 34))
styles.Add(new ElementStyle("Risk System", Background = "#550000", Color = "#ffffff"))

styles.Add(
    new ElementStyle(Tags.SoftwareSystem, Width = 650, Height = 400, Background = "#801515", Shape = Shape.RoundedBox)
)

styles.Add(new ElementStyle(Tags.Person, Width = 550, Background = "#d46a6a", Shape = Shape.Person))

styles.Add(new RelationshipStyle(Tags.Relationship, Thickness = 4, Dashed = false, FontSize = 32, Width = 400))
styles.Add(new RelationshipStyle(Tags.Synchronous, Dashed = false))
styles.Add(new RelationshipStyle(Tags.Asynchronous, Dashed = true))
styles.Add(new RelationshipStyle(AlertTag, Color = "#ff0000"))

styles.Add(new ElementStyle("Future State", Opacity = 30, Border = Border.Dashed))
styles.Add(new RelationshipStyle("Future State", Opacity = 30, Dashed = true))


let envVars = DotEnv.Read()

StructurizrClient(envVars.["STRUCTURIZR_API_KEY"], envVars.["STRUCTURIZR_SECRET"])
    .PutWorkspace(envVars.["STRUCTURIZR_WORKSPACE"] |> int64, workspace)
