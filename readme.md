# OPC-UA to IoT Central

Many factories have devices that speak [OPC-UA](https://opcfoundation.org/about/opc-technologies/opc-ua/) on their operational networks, however OPC-UA is not something that can directly be consumed by cloud services.  Often times devices that speak this protocl are also on isolated operational networks without internet access. However, in modern times we want this data visible in the cloud.  We want to build robust dashboards to monitor and alerts on data, and we want to be able to interact with these devices.

This repo shows a reference implementation for connecting OPC-Servers to [IoT Edge](https://azure.microsoft.com/en-us/services/iot-edge/) and then passing data up to IoT Central.

Setting all of this up end-to-end is a fairly simple process so **be sure to follow the [Quickstart guide](./quickstart.md) to get going.**


## IoT Edge Modules

There are three Edge modules used in this repository to demonstrate an end-to-end solution.

#### `opcSimulator`

This module leverages [Node-RED](https://nodered.org/) and the [node-red-contrib-opcua-server](https://flows.nodered.org/node/node-red-contrib-opcua-server) package to simulate a factory OPC-UA Server.  Two tags are available to query: `ITEM_COUNT_GOOD` and `ITEM_COUNT_BAD`.  We'll connect to this server with the OPC Publisher to collect data.

In a production implementation we would talk to an actual OPC Server and this simluator would not be deployed.

#### `opcPublisher`

The OPC Publisher module is part of the [Azure Industrial IoT](https://github.com/Azure/Industrial-IoT) Project and serves as a mechanism to listen to OPC Servers and publish the tag values out as JSON.

The OPC Publisher has two configuration modes: an API you can call to dynamically make changes, and a configuraiton file.  In this project we're leveraging the configuration file as it's simple and fits a workflow that most factories are comfortable with.  The configuration file is available here [publishedNodes.json](./modules/opcPublisher/publishedNodes.json) and containers the OPC Server information as well as all the tags we want to pull.

#### `opcToDtdl`

Many of the new Azure IoT Services, such as IoT Central, Azure Digital Twins, and the Time Series Insights Preview require data to conform to a well-defined model. This model is described using the open-source [Digital Twins Description Language](https://github.com/Azure/opendigitaltwins-dtdl/tree/master/DTDL), which is a core part of [IoT Plug & Play](https://docs.microsoft.com/en-us/azure/iot-pnp/overview-iot-plug-and-play).  

The data emitted by the OPC Publisher does not immediately conform data that can be described in this model so an additional module is used to make some simple transformations.

The `opcPublisher` emits data in this format:

```
{
    "NodeId": "OPC_NODE_ID",
    "ApplicationUri": "OPC_APPLICATION_URI",
    "DisplayName": "TAG_DISPLAY_NAME",
    "Status": "TAG_STATUS",
    "Value": {
        "Value": "TAG_VALUE",
        "SourceTimestamp: "TAG_SOURCE_TIMESTAMP"
    }
}
```

The `opcToDtdl` module is a simple [Azure Function](https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-deploy-function) takes this data and emits the following schema:

```
{
    "NodeId": "OPC_NODE_ID",
    "ApplicationUri": "OPC_APPLICATION_URI",
    "Status": "TAG_STATUS",
    "SourceTimestamp": "TAG_SOURCE_TIMESTAMP",
    "TAG_DISPLAY_NAME": "TAG_VALUE"
}
```

By using a key: value method for tag name and tag value we can easily modele this data in the DTDL spec, meaning it's compatible with all our moden Azure IoT Services.

As many pieces of the data sent may not come in exactly the format we want, options are available to parse `NodeId` and `ApplicationUri` with a regex as well.  You can specify these in module create options by setting the following environment variables: `NodeIdRegex`, `ApplicationUriRegex `.

This module is available as souce here but is also published to [Docker Hub](https://hub.docker.com/r/azureiotgbb/opc-publisher-to-dtdl).  The [production deployment template](./deployment.production.template.json) references the publisehd container images.

