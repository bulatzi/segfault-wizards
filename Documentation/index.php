<html>
<head>
<div>
	<h1>Login</h1>
</div>

</body>
</html>
<?php


	// Download xampp and replace the default index.php code with this

	/*
	Special conﬁguraon notes for PHP LDAP on Windows------
	
	NOTE: These instructions for PHP on Windows are VERY old and may no longer work.
	PHP uses the OpenLDAP library, which uses the OpenSSL library. You need to tell 
	OpenLDAP where to find the trusted root certificates. This is the correct way to 
	configure PHP LDAP for encryption on Windows. 
	
	1. Install PHP (recommended path is C:\php and version >= 5.3.1) 
	2. Make sure that libeay32.dll and ssleay32.dll are in the Windows path (typically C:\php) 
	3. Modify php.ini to tell PHP the locaon of the extensions directory (extension_dir = "c:\php\ext") 
	4. Modify php.ini to enable the LDAP extension (extension=php_ldap.dll) 
	5. Create C:\ldap.conf ﬁle (plain text, use Notepad or similar) 
	6. Add only the line TLS_CACERT c:\cacerts.pem to the C:\ldap.conf ﬁle 
	7. Create C:\cacerts.pem ﬁle (plain text, use Notepad or similar) 
	8. Obtain a copy of the USERTrust Secure cerﬁcate from InCommon : https://spaces.at.internet2.edu/display/ICCS/InCommon+Cert+Types
	9. Add that cerﬁcate (in PEM/Base64 encoded format) to the C:\cacerts.pem ﬁle 
	
	NOTE: You do not need to enable the PHP OpenSSL extension. Yes, the LDAP module 
	uses the OpenSSL code library, but that is not the same thing as the PHP extension for OpenSSL. 
	
	NOTE: The cacerts.pem file can contain multiple root certificates. You may wish
	to add the InCommon (a.k.a. Sectigo, Comodo) root ("USERTrust Secure") and the root
	certificate we use for Active Directory. 
	
	NOTE: Some of the comments on the PHP web site imply that PHP might look for the ldap.conf 
	file on a drive other than the C: drive, especially if your web server is installed on 
	another drive. It is not possible to relocate the ldap.conf file. In rare circumstances, 
	it may be necessary to use the FileMon utility to find out where PHP is looking. 
	Try these paths where X is the drive letter of any hard disk (starting with C:). 
	
	1. X:\ldap.conf 
	2. X:\OpenLDAP\sysconf\ldap.conf
	
	
	
	OpenSSL command reference------
	View certificate details (if needed, add "-inform DER" or "-inform PEM"): 
	openssl x509 -in input-file.pem -text 
	
	Convert certificate file format from binary (".cer") to Base64 (".pem"): 
	openssl x509 -inform DER -in input-file.cer -outform PEM -out output-file.pem 
	
	Convert certificate file format from Base64 (".pem") to binary (".cer"): 
	openssl x509 -inform PEM -in input-file.pem -outform DER -out output-file.cer
 

	
	*/
	
	//
	// Tutorial for connecting Angular with PHP:
	// https://phpenthusiast.com/blog/angular-php-app-creating-new-item-with-httpclient-post
	// Below is some of the code.
	//
	
	
	/**********************
	
	// Get the posted data.
	$postdata = file_get_contents("php://input");

	if(isset($postdata) && !empty($postdata))
	{
		// Extract the data.
		$request = json_decode($postdata);
	

		// Validate.
		if(trim($request->data->username) === '' || trim($request->data->password) === '')
		{
			return http_response_code(400);
		}
	}
		
	**********************/
		
		
	//
	//Test ldap Connection
	//Code from this video:
	//https://www.youtube.com/watch?v=AEjGhzZpGlg
	//
	
	/**********************
	
	$ldap_con = ldap_connect("ldap.forumsys.com");
	$ldap_dn = "cn=read-only-admin,dc=example,dc=com";
	$password = "password";
	ldap_set_option($ldap_con, LDAP_OPT_PROTOCOL_VERSION, 3);
	if(ldap_bind($ldap_con, $ldap_dn, $password))
	{
		echo "Bind Success";
	}
	else
	{
		echo "Invalid email address / password";
	}
	
	**********************/	
	
	
	//
	// Replace the below code to accept a username / password from Angular
	// instead of asking through PHP. The code may be working fine, but the 
	// setup for ldap connection may be wrong.
	//
	
	if(isset($_POST['username']) && isset($_POST['password']))
	{
		
		$username = $_POST['username'];
		$password = $_POST['password'];
		
		//Type in the address to connect to
		$ldap_con = ldap_connect("ldap.example.com");
		
		//Type in the search base
		$ldap_dn = "cn=XXXX,ou=XXXX,o=XXXX";
		
		
		if(ldap_set_option($ldap_con, LDAP_OPT_PROTOCOL_VERSION, 3))
		{
			if(ldap_set_option($ldap_con, LDAP_OPT_REFERRALS, 0))
			{
				//TLS Encryption
				if(ldap_start_tls($ldap_con))
				{
					//Binding will work with the correct credentials and if
					//everything else is working
					if(ldap_bind($ldap_con, $ldap_dn, $password))
					{
						echo "Bind Success";
					}
					else
					{
						echo "Invalid email address / password";
					}	
				}
			}
		}
		
		ldap_close($ldap_con);
	}
	else
	{
?>
    <form action="#" method="POST">
        <label for="username">Username: </label><input id="username" type="text" name="username" /> 
        <label for="password">Password: </label><input id="password" type="password" name="password" />        <input type="submit" name="submit" value="Submit" />
    </form>
<?php } ?> 

<html>
<head>
<div>
	<a class="ic-Login__link not_external" id="login_forgot_password" href="https://ams.unt.edu">Forgot Password?</a>
</div>

</body>
</html>

<?php
	phpinfo()
?>