<?php
// IF YOU END UP USING THIS PLEASE DON'T USE MY LOGIN SYSTEMS AS THEY ARE NOT 100% SECURE
// IF YOU END UP USING THIS PLEASE DON'T USE MY LOGIN SYSTEMS AS THEY ARE NOT 100% SECURE
// IF YOU END UP USING THIS PLEASE DON'T USE MY LOGIN SYSTEMS AS THEY ARE NOT 100% SECURE
// IF YOU END UP USING THIS PLEASE DON'T USE MY LOGIN SYSTEMS AS THEY ARE NOT 100% SECURE


if($_POST['hash'] == "aweqwrey:Pre-Alpha V0.0.4") {
	
	$servername = "localhost"; // Set to this cuz I needed to hide it?
	$username = "root"; // Set to this cuz I needed to hide it?
	$password = ""; // Set to this cuz I needed to hide it?
	$dbname = "ProjectTTT";
					
	$mysqli = new mysqli($servername, $username, $password, $dbname);

	if ($mysqli->connect_error) {
		//die("Connection failed: " . $mysqli->connect_error);
		die();
	}
	
	if ($stmt = $mysqli->prepare("SELECT id, password FROM accounts WHERE username=?")) {

		$stmt->bind_param("s", $_POST['uname']);
		$stmt->execute();
		$res = $stmt->get_result();
		$countRows = $res->num_rows;
		$row = $res->fetch_assoc();
		
		if($countRows == 0) {
			echo "LoginFailed2";
		} else {
			$hash = $row['password'];
			$id = $row['id'];
				
			if (password_verify($_POST['pwd'], $hash)) {
				echo "LoginCorrect";
			} else {
				echo "LoginFailed";
			}
		}
	} else {
		echo "LoginFailed1";
	}

	$stmt->close();
	$mysqli->close();
} else {
	die();
}
?>