#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: dotenv.net, 3.0.0"
#r "nuget: Structurizr.Core, 1.0.0"
#r "nuget: Structurizr.Client, 1.0.0"

open dotenv.net
open Structurizr
open Structurizr.Api


let workspace =
    Workspace("Documentation - arc42", "An empty software architecture document using the arc42 template.")

let model = workspace.Model
let views = workspace.Views

let user =
    model.AddPerson("User", "A user of my system")

let system =
    model.AddSoftwareSystem("Software System", "My system.")

user.Uses(system, "Uses")

let ctxView =
    views.CreateSystemContextView(system, "system context", "system context diagram")

ctxView.AddAllPeople()
ctxView.AddAllSoftwareSystems()

let styles = views.Configuration.Styles
styles.Add(ElementStyle(Tags.SoftwareSystem, Background = "#1168bd", Color = "#ffffff"))
styles.Add(ElementStyle(Tags.Person, Background = "#08427b", Color = "#ffffff", Shape = Shape.Person))

let envVars = DotEnv.Read()

StructurizrClient(envVars.["STRUCTURIZR_API_KEY"], envVars.["STRUCTURIZR_SECRET"])
    .PutWorkspace(envVars.["STRUCTURIZR_WORKSPACE"] |> int64, workspace)
