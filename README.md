<h1>Lead Ad Data Retrieve Application</h1>
<div>LeadRetrieve is an application designed to retrieve lead data from Facebook Ads using webhooks and process it for further analysis. This application integrates with the Facebook graph API to fetch leads and store them in a database.
Table of Contents</div>

<h3>Prerequisites</h3>
- Ensure you have the following installed:</br>
.NET Core SDK 6.0+</br>
Docker (optional, for containerization)</br>
PostgreSQL (or any preferred database)</br>
Facebook Developer account with a Facebook App setup for Lead Ads</br>
Ngrok for local development (if testing webhooks)</br>


<h3>Implementation</h3>
-Compile and host the Web API in your server Facebook webhooks only accepts https URL

<h3>Facebook App & Webhook Setup</h3>
Create facebook app</br>
Add webhooks to the app product</br>
Config webhook--> select page</br>
subscribe to Leadgen --> test</br>
request post api "<user_id>/subscribed_apps?subscribed_fields=leadgen" to subcribe leadgen webhook for the page</br>


<h3>Environment Setup</h3>
-You need to configure your environment variables in a .env file in the project root director</br>
Note: Make sure the .env file is included and properly loaded when building the application with Docker or running locally.

<h3>Testing with Ngrok</h3>
-If you are developing locally, use Ngrok to expose your local server to the internet:
Add ngrok_authtoken to .env

<div>NOTE:  Lead Ads Testing tool does not include the ad_id or adgroup_id when request POST webhook</div>
<h2>License</h2>
This project is licensed under the MIT License.
