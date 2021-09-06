{
	function submit(event)
	{
		event.preventDefault();
		//console.log(event.target.elements);
		log.textContent = `Form Submitted! Time stamp: ${event.timeStamp}`;
		var formdata = new FormData(event.target);
		var qmutation = event.target.getAttribute("graphql_mutation");
		var query = 'mutation{' + qmutation + '(' + gql_formdata_string(formdata) + '){id}}';
		var cbname = event.target.getAttribute("callback");
		//console.log(cbname, q);


		var xhr = new XMLHttpRequest();
		xhr.open("POST", window.location.origin + "/graphql", true);
		xhr.setRequestHeader("Content-Type", "application/json");
		xhr.onload = function ()
		{
			var r = JSON.parse(xhr.response);
			if (r.data === null) { return; }
			window[cbname](r.data);
		};
		xhr.onerror = function ()
		{
			console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
		}
		var data = JSON.stringify({ "query": query });
		xhr.send(data);

	}

	let elements_form = document.querySelectorAll('form[graphql_mutation]');
	for (let i = 0; i < elements_form.length; ++i)
	{
		elements_form[i].addEventListener('submit', submit);
	}
}