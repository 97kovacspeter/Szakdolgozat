import socket
import time
import random

connection = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# set timeout 5 second
connection.settimeout(1)
server_address = ('52.236.157.144', 8000)

while True:
	data="8002|receiver"
	connection.sendto(data.encode(), server_address)
	print("8002|receiver sent to server")
	try:
		data, client_addr = connection.recvfrom(50000)
		print(data.strip().decode())
	except:
		print("Timeout")
		continue

connection.close()