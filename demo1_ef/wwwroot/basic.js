const assert = function(condition, message)
{
    if (condition === false)
	{
		throw Error('Assert failed: ' + (message || ''));
	}
};


function ws_url(postfix)
{
	var scheme = document.location.protocol === "https:" ? "wss" : "ws";
	var port = document.location.port ? (":" + document.location.port) : "";
	var url = scheme + "://" + document.location.hostname + port + postfix;
	return url;
}


function table_insert_th(tr) {
	var th = document.createElement('th');
	tr.appendChild(th);
	return th;
}

function formatdate(t) {
	var Y = t.getFullYear();
	var M = t.getMonth();
	var D = t.getDate();
	var h = t.getHours();
	var m = t.getMinutes();
	var s = t.getSeconds();
	return Y + '' + M + '' + D + ' ' + h + ':' + m + ':' + s;
}



function template(text, data) {
	return text.replace(
		/{(\w*)}/g, // or /{(\w*)}/g for "{this} instead of %this%"
		function (m, key) {
			return data.hasOwnProperty(key) ? data[key] : "";
		}
	);
}



function xmur3(str) {
	for (var i = 0, h = 1779033703 ^ str.length; i < str.length; i++)
		h = Math.imul(h ^ str.charCodeAt(i), 3432918353),
			h = h << 13 | h >>> 19;
	return function () {
		h = Math.imul(h ^ h >>> 16, 2246822507);
		h = Math.imul(h ^ h >>> 13, 3266489909);
		return (h ^= h >>> 16) >>> 0;
	}
}


function sfc32(a, b, c, d) {
	return function () {
		a >>>= 0; b >>>= 0; c >>>= 0; d >>>= 0;
		var t = (a + b) | 0;
		a = b ^ b >>> 9;
		b = c + (c << 3) | 0;
		c = (c << 21 | c >>> 11);
		d = d + 1 | 0;
		t = t + d | 0;
		c = c + t | 0;
		return (t >>> 0) / 4294967296;
	}
}


function mulberry32(a) {
	return function () {
		var t = a += 0x6D2B79F5;
		t = Math.imul(t ^ t >>> 15, t | 1);
		t ^= t + Math.imul(t ^ t >>> 7, t | 61);
		return ((t ^ t >>> 14) >>> 0) / 4294967296;
	}
}


function xoshiro128ss(a, b, c, d) {
	return function () {
		var t = b << 9, r = a * 5; r = (r << 7 | r >>> 25) * 9;
		c ^= a; d ^= b;
		b ^= c; a ^= d; c ^= t;
		d = d << 11 | d >>> 21;
		return (r >>> 0) / 4294967296;
	}
}


function jsf32(a, b, c, d) {
	return function () {
		a |= 0; b |= 0; c |= 0; d |= 0;
		var t = a - (b << 27 | b >>> 5) | 0;
		a = b ^ (c << 17 | c >>> 15);
		b = c + d | 0;
		c = d + t | 0;
		d = a + t | 0;
		return (d >>> 0) / 4294967296;
	}
}


function random_hsla(seed)
{
	var r = mulberry32(xmur3(seed)())();
	return 'hsla(' + (r * 360) + ', 100%, 80%, 1)';
}


function random_hsla2(seed)
{
	var r = mulberry32(xmur3(seed)())();
	var a = 'hsla(' + (r * 360) + ', 100%, 80%, 1)';
	var b = 'hsla(' + (r * 180) + ', 100%, 80%, 1)';
	return [a, b];
}

function buf2hex(buffer)
{
	// buffer is an ArrayBuffer
	return Array.prototype.map.call(new Uint8Array(buffer), x => ('00' + x.toString(16)).slice(-2)).join('');
}


function gql_request(q, onload)
{
	var xhr = new XMLHttpRequest();
	xhr.open("POST", window.location.origin + "/graphql", true);
	xhr.setRequestHeader("Content-Type", "application/json");
	xhr.onload = function ()
	{
		var r = JSON.parse(xhr.response);
		if (r.data === null){return;}
		if (onload != null)
		{
			console.log("request", q, "response", r.data);
			onload(r.data);
		}
	};
	xhr.onerror = function()
	{
		console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
	}
	var data = JSON.stringify({"query": q});
	xhr.send(data);
}



function gql_floatval_add(value, onload)
{
	gql_request("mutation{floatval_add(value:"+value+")}", onload);
}