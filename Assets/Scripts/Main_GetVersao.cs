using UnityEngine;
using System.Collections;

public class Main_GetVersao : MonoBehaviour {
	[SerializeField] private string chave;
	[SerializeField] private int versao;
	[SerializeField] private string verificarUrl;
	[SerializeField] private string nomeUrl;
	private bool  chamei = false;

	void Start (){
		StartCoroutine(verificar(versao));
		
		if(PlayerPrefs.GetString("nome") == "" || PlayerPrefs.GetInt("nomePendente") != 0 || !PlayerPrefs.HasKey("nome")) {
			Debug.Log("Nome não encontrado.");
			if(SystemInfo.deviceName.Contains("-PC")) {
				string[] meuNome;
				meuNome = SystemInfo.deviceName.Split("-PC"[0]);
				StartCoroutine(criaNome(meuNome[0]));
				chamei = true;
			}
			else {
				StartCoroutine(criaNome(SystemInfo.deviceName));
				chamei = true;
			}
		}
	}

	void OnGUI (){
		GUI.color = new Color(1, 1, 1, 0.1f);
		GUI.Label(new Rect(Screen.width - 50, 10, 220, 45), "0." + versao);
	}

	IEnumerator verificar ( int ver  ){ 
		string verUrl= verificarUrl + ver + "&plataforma=" + Application.platform;
	    WWW checar = new WWW(verUrl);
	    Debug.Log("Verificando versão: " + verUrl);
	    yield return checar;

	    if(checar.error != null) {
	    	Debug.Log("Não foi possível verificar a versão.");
	        SendMessage("podeJogar", "");
	    } else if(checar.text.Contains("ok")) {
	    	Debug.Log("Versão OK.");
	    	SendMessage("podeJogar", "");
	    } else {
	    	Debug.Log("Versão desatualizada.");
	    	SendMessage("podeJogar", checar.text);
	    }
	}



	IEnumerator criaNome ( string oNovoNome  ){
		bool  igual = true;
		int id = 0;
		if(oNovoNome == "<unknown>" || oNovoNome == "n/a" || oNovoNome == "")
			oNovoNome = "Super_Jogador";
		Debug.Log("Vou escolher o nome.");
		string novoNome = oNovoNome;
		while(igual) {
			if(id > 0)
				novoNome = oNovoNome + "_" + id;
			
			string hash = md5.Md5Sum(novoNome + SystemInfo.deviceUniqueIdentifier + chave);
			// Acessa o arquivo no servidor e verifica o nome
		    string verifica = nomeUrl + WWW.EscapeURL(novoNome) + "&id=" + SystemInfo.deviceUniqueIdentifier + "&hash=" + hash;
		    // Cria o objeto que faz o processo
		    WWW verificando = new WWW(verifica);
		    
		    Debug.Log("Processando o pedido para '" + novoNome + "'... (" + verifica + ")");
		    yield return verificando; // Espera a resposta
		    
		    if(verificando.error != null) {
		    	igual = false;

	    		PlayerPrefs.SetString("nome", novoNome);
				SendMessage("trocaNome", novoNome);

		    	if(!chamei) {
					SendMessage("foiSucesso", true);
				}
				
		        PlayerPrefs.SetInt("nomePendente", 1);
		        
		        Debug.Log("Sem conexao com a internet. Usando nome generico. Erro: " + verificando.error);
		    }
		    else if(chamei && verificando.text.Contains("sim")) {
				SendMessage("foiSucesso", false);
				Debug.Log("Ja existe um usuario com este nome.");
				break;
		    }
		    else {
		    	igual = false;
		    	PlayerPrefs.SetString("nome", novoNome);
				SendMessage("trocaNome", novoNome);
		    	PlayerPrefs.SetInt("nomePendente", 0);
		    	Debug.Log("Criei o nome: " + novoNome);
		    	if(!chamei) {
					SendMessage("foiSucesso", true);
				}
		    }	    
		    id++;
		}
		chamei = false;
	}
}