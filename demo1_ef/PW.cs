using System;
using System.Data;
using System.Data.Common;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Serilog;

using Npgsql;
using NpgsqlTypes;


//https://www.codeproject.com/Articles/5263745/Return-DataTable-Using-Entity-Framework

namespace Demo
{
	public static class PW
	{

		static private readonly int sn = 16;
		static private readonly int pn = 20;
		static private readonly int iterations = 100000;

		//Generated salthash. Password is secret.
		static public readonly byte[] salthash0 = {0x1F,0xFB,0x4F,0x32,0x70,0xBB,0x97,0x12,0x3A,0x7A,0x7D,0x88,0xF1,0x21,0x6B,0x79,0x83,0x6A,0xD7,0xBC,0xDB,0x57,0xD8,0x6F,0x35,0x29,0x0C,0x1C,0xAF,0x07,0x91,0x30,0x5C,0x97,0x4D,0xE9};

		public static byte[] salthash_create(string password)
		{
			return Password.salthash_create(password, sn, pn, iterations);
		}

		public static bool salthash_verify(string password, byte[] salthash)
		{
			return Password.salthash_verify(password, salthash, sn, pn, iterations);
		}
	}
}