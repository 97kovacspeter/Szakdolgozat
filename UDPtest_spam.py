import socket
import time

connection = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
server_address = ('52.236.157.144', 8000)

while True:
	for i in range(20):
		time.sleep(1)
		data="********************************"
		connection.sendto(data.encode(), server_address)
		print("******************************** sent to server")
	data="8001|sender"
	connection.sendto(data.encode(), server_address)
	print("8001|sender sent to server")
	time.sleep(1)
	data="8001|sender"
	connection.sendto(data.encode(), server_address)
	print("8001|sender sent to server")
	time.sleep(1)
	data="8001|receiver"
	connection.sendto(data.encode(), server_address)
	print("8001|receiver sent to server")
	time.sleep(1)
	data="********************************"
	connection.sendto(data.encode(), server_address)
	print("******************************** sent to server")
	for i in range(20):
		time.sleep(1)
		data="********************************"
		connection.sendto(data.encode(), server_address)
		print("******************************** sent to server")
	time.sleep(10)
	data="8001|receiver"
	connection.sendto(data.encode(), server_address)
	print("8001|receiver sent to server")




connection.close()