using System;
using System.Diagnostics;
using System.Security.Cryptography;

public static class Password
{

	//
	// Summary:
	//     Creates a byte array that contains both salt and hash values.
	//
	// Parameters:
	//   password:
	//     The password to hash.
	//   sn:
	//     The number of cryptographically strong sequence of random values for salt part. Must by atleast 8.
	//   pn:
	//     The number of pseudo-random key bytes to generate for hash part. Must be positive.
	//   n:
	//     The number of iterations for the operation.
	//
	// Returns:
	//     A byte array of salt and hashed values. (S1, S2, ... sn, H1, H2, ... pn)
	public static byte[] salthash_create(string password, int sn, int pn, int n)
	{
		byte[] salt;
		new RNGCryptoServiceProvider().GetBytes(salt = new byte[sn]);
		//PBKDF2 Password-Based Key Derivation Function 2
		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, n);
		byte[] pwhash = pbkdf2.GetBytes(pn);
		byte[] h = new byte[sn+pn];
		Array.Copy(salt, 0, h, 0, sn);
		Array.Copy(pwhash, 0, h, sn, pn);
		return h;
	}

	//
	// Summary:
	//     Compares raw password with hashed password.
	//
	// Parameters:
	//   pwhash:
	//     The salt and hashed password to compare. (Salt, Hash) = (S1, S2, ... sn, H1, H2, ... pn)
	//   password:
	//     The raw password to compare.
	//   sn:
	//     The number of cryptographically strong sequence of random values. Must by atleast 8.
	//   pn:
	//     The number of pseudo-random key bytes to generate. Must be positive.
	//   n:
	//     The number of iterations for the operation.
	//
	// Returns:
	//     Matched or not matched password
	public static bool salthash_verify(string password, byte[] pwhash, int sn, int pn, int n)
	{
		byte[] salt = new byte[sn];
		Array.Copy(pwhash, 0, salt, 0, sn);
		//PBKDF2 Password-Based Key Derivation Function 2
		var pbkdf2 = new Rfc2898DeriveBytes(password, salt, n);
		byte[] hash = pbkdf2.GetBytes(pn);
		for (int i = 0; i < pn; i++)
		{
			if (pwhash[i+sn] != hash[i])
			{
				return false;
			}
		}
		return true;
	}


	public static void assert_salthash()
	{
		string pw1 = "abc";
		string pw2 = "abC";
		byte[] hash;

		hash = salthash_create(pw1, 8, 1, 1);
		Debug.Assert(salthash_verify(pw1, hash, 8, 1, 1) == true);
		Debug.Assert(salthash_verify(pw2, hash, 8, 1, 1) == false);

		hash = salthash_create(pw1, 8, 2, 1);
		Debug.Assert(salthash_verify(pw1, hash, 8, 2, 1) == true);
		Debug.Assert(salthash_verify(pw2, hash, 8, 2, 1) == false);

		hash = salthash_create(pw1, 8, 100, 1);
		Debug.Assert(salthash_verify(pw1, hash, 8, 100, 1) == true);
		Debug.Assert(salthash_verify(pw2, hash, 8, 100, 1) == false);
	}



}