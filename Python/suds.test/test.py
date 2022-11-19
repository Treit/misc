from suds.client import Client
client = Client("http://127.0.0.1:5125/RandomNumber.svc?singleWsdl")
print(client)

headers = {
    'Max': 100
 }

client.set_options(soapheaders=headers)
result = client.service.Next()

print(result)
