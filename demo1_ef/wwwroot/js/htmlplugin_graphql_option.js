{
	let elements_options = document.querySelectorAll('select[graphql_option]');
	if (elements_options.length > 0)
	{

		let elements_options1 = {};
		let query = "query{\n";
		for (let i = 0; i < elements_options.length; ++i)
		{
			const name = elements_options[i].getAttribute("name");
			const q = elements_options[i].getAttribute("graphql_option");
			elements_options1[name] = elements_options[i];
			query += name + ":" + q + "\n";
		}
		query += "}";
		//console.log(elements_options1, query);
		{
			var xhr = new XMLHttpRequest();
			xhr.open("POST", window.location.origin + "/graphql", true);
			xhr.setRequestHeader("Content-Type", "application/json");
			xhr.onload = function ()
			{
				var r = JSON.parse(xhr.response);
				if (r.data === null) { return; }
				for (const property in r.data)
				{
					//console.log(property, r.data[property]);
					for (i = 0; i < r.data[property].length; ++i)
					{
						var element_option = document.createElement('option');
						element_option.value = r.data[property][i].value;
						element_option.textContent = r.data[property][i].name + (r.data[property][i].name1 ? " (" + r.data[property][i].name1 + ")" : "");
						elements_options1[property].appendChild(element_option);
					}
				}
			};
			xhr.onerror = function ()
			{
				console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
			}
			var data = JSON.stringify({ "query": query });
			xhr.send(data);
		}
	}
}