<?php
// IF YOU END UP USING THIS PLEASE DON'T USE MY LOGIN SYSTEMS AS THEY ARE NOT 100% SECURE
// IF YOU END UP USING THIS PLEASE DON'T USE MY LOGIN SYSTEMS AS THEY ARE NOT 100% SECURE
// IF YOU END UP USING THIS PLEASE DON'T USE MY LOGIN SYSTEMS AS THEY ARE NOT 100% SECURE
// IF YOU END UP USING THIS PLEASE DON'T USE MY LOGIN SYSTEMS AS THEY ARE NOT 100% SECURE


$servername = "localhost"; // Set to this cuz I needed to hide it?
$username = "root"; // Set to this cuz I needed to hide it?
$password = ""; // Set to this cuz I needed to hide it?
$dbname = "ProjectTTT";
				
$mysqli = new mysqli($servername, $username, $password, $dbname);

if ($mysqli->connect_error) {
	//die("Connection failed: " . $mysqli->connect_error);
	die();
}

if(isset($_POST['register'])) {
	$pwd = password_hash($_POST['password'], PASSWORD_DEFAULT);
	if ($stmt = $mysqli->prepare("SELECT username, email FROM accounts WHERE username=? or email=?")) {

		$stmt->bind_param("ss", $_POST['username'],$_POST['email']);
		$stmt->execute();
		$stmt->store_result();

		if($_POST['username'] == "" || $_POST['email'] == "" || $_POST['password'] == "") {
			die("Blank Field Found!");
		} else {
			if($stmt->num_rows > 0) {
				die("Email or Username already in use");
			} else {
				if ($stmt2 = $mysqli->prepare("INSERT INTO accounts (username, email, password) VALUES (?, ?, ?)")) {

					$stmt2->bind_param("sss", $_POST['username'],$_POST['email'],$pwd);

					$stmt2->execute();
					
					$stmt2->close();
				}
			}
		}
		$stmt->close();
	}
	
	$mysqli->close();
}
?>
<html>

	<head>
	
	</head>
	
	<body>
		<h2>Placeholder Register</h2>
		<p>Used to create your own account on ProjectTTT.</p>
		<form class="form-horizontal" action="" method="POST">
			<input type="text" name="register" hidden>
			<div class="form-group">
			  <label class="control-label col-sm-2" for="username">Username:</label>
			  <div class="col-sm-10">
				<input type="text" class="form-control" id="username" placeholder="Enter username" name="username">
			  </div>
			</div>
			<div class="form-group">
			  <label class="control-label col-sm-2" for="email">Email:</label>
			  <div class="col-sm-10">
				<input type="email" class="form-control" id="email" placeholder="Enter email" name="email">
			  </div>
			</div>
			<div class="form-group">
			  <label class="control-label col-sm-2" for="password">Password:</label>
			  <div class="col-sm-10">          
				<input type="password" class="form-control" id="password" placeholder="Enter password" name="password">
			  </div>
			</div>
			<div class="form-group">        
			  <div class="col-sm-offset-2 col-sm-10">
				<button type="submit" class="btn btn-default">Sign Up</button>
			  </div>
			</div>
		</form>
	</body>
</html>