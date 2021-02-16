import socket
import time

connection = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
server_address = ('52.236.157.144', 8000)

while True:
	data="8002|sender"
	connection.sendto(data.encode(), server_address)
	print("8000|sender sent to server")
	time.sleep(1)
	data="asgdhfasgASFDdfasgdhfasgASFDdf"
	connection.sendto(data.encode(), server_address)
	print("asgdhfasgASFDdfasgdhfasgASFDdf sent to server")
	for i in range(20):
		time.sleep(1)
		data="asgdhfasgASFDdfasgdhfasgASFDdf"
		connection.sendto(data.encode(), server_address)
		print("asgdhfasgASFDdfasgdhfasgASFDdf sent to server")
	time.sleep(10)
	data="FFFFFF"
	connection.sendto(data.encode(), server_address)
	print("FFF sent to server")




connection.close()
