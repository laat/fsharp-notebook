#!/usr/bin/env -S dotnet fsi --quiet
#r "nuget: dotenv.net, 3.0.0"
#r "nuget: Structurizr.Core, 1.0.0"
#r "nuget: Structurizr.Client, 1.0.0"

open dotenv.net
open Structurizr
open Structurizr.Api

let springBootTag = "Spring Boot Application"
let databaseTag = "Database"

let workspace =
  Workspace("Amazon Web Services Example", "An example AWS deployment architecture.")

let model = workspace.Model

let softwareSystem =
  model.AddSoftwareSystem(
    "Spring PetClinic",
    "Allows employees to view and manage information regarding the veterinarians, the clients, and their pets."
  )

let webApplication =
  softwareSystem.AddContainer(
    "Web Application",
    "Allows employees to view and manage information regarding the veterinarians, the clients, and their pets.",
    "Java and Spring Boot"
  )
webApplication.AddTags(springBootTag)

let database =
  softwareSystem.AddContainer(
    "Database",
    "Stores information regarding the veterinarians, the clients, and their pets.",
    "Relational database schema"
  )
database.AddTags(databaseTag);

webApplication.Uses(database, "Reads from and writes to", "JDBC/SSL");

let amazonWebServices = model.AddDeploymentNode("Amazon Web Services")
amazonWebServices.AddTags("Amazon Web Services - Cloud")

let amazonRegion = amazonWebServices.AddDeploymentNode("US-East-1");
amazonRegion.AddTags("Amazon Web Services - Region")

let autoscalingGroup = amazonRegion.AddDeploymentNode("Autoscaling group")
autoscalingGroup.AddTags("Amazon Web Services - Auto Scaling");

let ec2 = autoscalingGroup.AddDeploymentNode("Amazon EC2")
ec2.AddTags("Amazon Web Services - EC2");

let webApplicationInstance = ec2.Add(webApplication);
let route53 = amazonRegion.AddInfrastructureNode("Route 53");
route53.AddTags("Amazon Web Services - Route 53")

let elb = amazonRegion.AddInfrastructureNode("Elastic Load Balancer")
elb.AddTags("Amazon Web Services - Elastic Load Balancing")

route53.Uses(elb, "Forwards requests to", "HTTPS");
elb.Uses(webApplicationInstance, "Forwards requests to", "HTTPS");

let rds = amazonRegion.AddDeploymentNode("Amazon RDS");
rds.AddTags("Amazon Web Services - RDS")

let mySql = rds.AddDeploymentNode("MySQL")
mySql.AddTags("Amazon Web Services - RDS_MySQL_instance");

let databaseInstance = mySql.Add(database);

let views = workspace.Views
let deploymentView = views.CreateDeploymentView(softwareSystem, "AmazonWebServicesDeployment", "An example deployment diagram.");
deploymentView.AddAllDeploymentNodes();
deploymentView.AddAnimation(route53);
deploymentView.AddAnimation(elb);
deploymentView.AddAnimation(webApplicationInstance);
deploymentView.AddAnimation(databaseInstance);

let styles = views.Configuration.Styles
styles.Add(ElementStyle(springBootTag, Shape = Shape.RoundedBox, Background = "#ffffff" ))
styles.Add(ElementStyle(databaseTag, Shape = Shape.Cylinder, Background = "#ffffff"))
styles.Add(ElementStyle(Tags.InfrastructureNode, Shape = Shape.RoundedBox, Background = "#ffffff"))

views.Configuration.Theme = "https://raw.githubusercontent.com/structurizr/themes/master/amazon-web-services/theme.json";

let envVars = DotEnv.Read()

StructurizrClient(envVars.["STRUCTURIZR_API_KEY"], envVars.["STRUCTURIZR_SECRET"])
  .PutWorkspace(envVars.["STRUCTURIZR_WORKSPACE"] |> int64, workspace)
