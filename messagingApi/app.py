#!flask/bin/python
from twilio.rest import TwilioRestClient
from flask import Flask, request
from twilio import twiml
import requests

account_sid = "AC501288d47afc284142feded345c61a0e" # My specific account ID from twilio
auth_token  = "786c97bcb800f367d67e9a2bd882c58a"  # My specific authentication token from Twilio

client = TwilioRestClient(account_sid, auth_token) # Setup object for Twilio API

    

app = Flask(__name__)

@app.route('/', methods = ['GET']) # Test function - useful to test if the API is working (returns Hello World)
def helloWorld():
    return "Hello World!"

@app.route('/sms', methods = ['GET','POST']) #This is the function which the Twilio API sends a message to if a message is received on the number
def index():
        number = request.form['From'] # Gets the number from which it is sent
        message_body = request.form['Body'] #Gets the body of the text
        response = twiml.Response()
        headers = {'content-type': 'application/json'}
        url = "http://localhost:5000/api/app/sms" + number + "," + message_body
        r = requests.post(url) # Sends a message to the WebAPI with the number and contents of the message.
        response.message('Thanks for your request. Your appointment details will be sent to you shortly') # Also responds to the message directly (in case the WebAPI is broken or takes a while)
        return str(response)

@app.route('/sendSms', methods = ['POST']) #When called by the WebAPI
def sendMessage():
        data = request.json 
        number = data['From'] # Parses the JSON and gets the number and message_body of the text to be sent
        message_body = data['Body']
        message = client.messages.create(body=message_body, 
            to=number,    
            from_="+441202237638") #Defines the message
        return "SUCCESS" # Returns a string "SUCCESS" instead of a boolean as the Flask API does not allow for returning booleans
    
@app.route('/verifyNumber', methods = ['POST']) # When called by the Android Application
def verifyNumber():
    data = request.json # Parses the JSON object into number and friendlyName
    number = data['Number']
    friendlyName = data['friendlyName']
    caller_id = client.caller_ids.validate("+" + number, friendly_name = friendlyName) # Sends request to the Twilio API and gets the validation code
    a = caller_id ['validation_code'] 
    return a # returns the validation code


if  __name__ == '__main__': # Entry point for application
	app.debug=True #Allows things to be printed to the screen
	app.run(port=5001) # Begins the listener
