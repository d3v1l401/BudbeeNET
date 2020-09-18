# Budbee NET

**This is not an official API nor I'm associated with the company, this is purely possible as their APIs allows so.**

BudbeeNET is no more no less than the API client for Budbee delivery service being written in C#.

  - Retrieve full package/delivery information.
	- Including driver's name, photo and GPS position as of now.
  - Retrieve sender's data (Banner) to classify deliveries based on product itself.

This tool has been written by [d3vil401][d3vsite].

### Usage

These are the arguments:

```sh
-o <delivery id> : The delivery ID given to you by SMS/Mail.
-b : Also retrieve information about the sender.
-t : Also track the driver's GPS location in real time (given the backend allows so).
```

![Demo](https://x)

#### Retrieve the Delivery ID

When you receive a message from Budbee or a delivery notification, you get a link to their tracking tool.
Use the alphanumeric string after the `/` but skip the `auth` parameter.

`https://tracking.budbee.com/<deliveryId>`


   [d3vsite]: <https://d3vsite.org>
