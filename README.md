# ChatBot_with_Automation

**Overview:** This project is to demonstrate how the Robotic Process Automation(RPA) and ChatBots can be integrated together.

**Tools Used:**
  1. DialogFlow
  2. UiPath
  3. Visual Studio 2019
  4. Microsoft Azure

**Workflow of the Project:** The working of the project is as follows, First, the user interacts with the chatbot [here I use Google's DialogFlow for chatbot] and describes his needs by entering the text or by making voice commands to the chatbot, after that Chabot takes the text or voice data given by the user and extract the relevant information from it, after that accordingly the chatbot triggers a robotic process automation [here I use UiPath for RPA] and the result or the information provided by that RPA will be provided back to the user on the chatbot. 

**Technical Description:** Whenever the user types anything or says anything to the chatbot, it takes all the data from the user and extract the relevant information from it, after that DialogFlow sends a Post response with all the extracted information to a Webhook [here I use .net and C# to write the Webhook code and hosted it on Microsoft Azure], after that, the Webhook performs all the related activities and triggers a UiPath process using UiPath Orchestrator APIs which starts the UiPath robot deployed to a machine and performs the automated task and generated a response which is sent back to the webhook and from the webhook again the result is sent on the chatbot. 
