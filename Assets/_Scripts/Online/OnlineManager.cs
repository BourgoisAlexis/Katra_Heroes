using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using PlayerIOClient;
using UnityEngine.SceneManagement;

public enum e_status
{
	Default,

	InSelection,
	InGame,
	GameEnded
}

public class OnlineManager : MonoBehaviour
{
	#region Variables
	private string userID;
	private string roomID;
	private Connection connection;
	private Client client;

	private List<RoomInfo> roomInfos = new List<RoomInfo>();
	private List<Message> messages = new List<Message>();

	private e_status currentStatus;

	//PlayerJoin
	private bool teamAttributed;
	private e_teams team;

	//PlayerReady
	private int[] heroes = new int[6];
	private AsyncOperation loadGameplay;

	//Gameplay
	private GameplayManager gameplayManager;
	#endregion


	//Initialization
	private void Start()
	{
		SceneTransi.Instance.Transi(false, null);

		currentStatus = e_status.Default;
		DontDestroyOnLoad(this);
		Connect();
	}

	public void Connect()
	{
		userID = Random.Range(0, 2000).ToString();

		//Application.runInBackground = true;

		PlayerIO.Authenticate
		(
			"katra-heroes-4gxpd8bc9egeca8jtx5xzq",                          //Your game id
			"public",                                                       //Your connection id
			new Dictionary<string, string> { { "userId", userID }, },       //Authentication arguments
			null,                                                           //PlayerInsight segments
			(Client _client) => { AuthentSuccess(_client); },
			(PlayerIOError _error) => { Fail(_error, "Connect"); }
		);
	}

	private void AuthentSuccess(Client _client)
	{
		Debug.Log("Successfully connected to Player.IO");
		client = _client;

		Debug.Log("Create ServerEndpoint");
		// Comment out the line below to use the live servers instead of your development server
		client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);
	}


	//Join
	public void Play()
	{
		client.Multiplayer.ListRooms
		(
			"Battle",
			null,
			100,
			0,
			(RoomInfo[] _infos) => { ListRoomsJoin(_infos); },
			(PlayerIOError _error) => { Fail(_error, "Play"); }
		);
	}

	private void ListRoomsJoin(RoomInfo[] _infos)
	{
		roomID = string.Empty;

		if (_infos.Length < 1)
		{
			CreateRoom();
			return;
		}
		else
		{
			roomInfos.Clear();
			roomInfos.AddRange(_infos);

			for (int i = roomInfos.Count - 1; i >= 0; i--)
			{
				Debug.Log(i);

				if (roomInfos[i].OnlineUsers < 2)
				{
					if (roomID == string.Empty)
					{
						roomID = roomInfos[i].Id;
						roomInfos.RemoveAt(i);
						break;
					}
				}
				
				roomInfos.RemoveAt(i);
			}

			if (roomID == string.Empty)
			{
				CreateRoom();
				return;
			}
		}

		JoinRoom();
	}

	private void ListRoomsCreate(RoomInfo[] _infos)
	{
		bool same = true;

		while (same)
		{
			same = false;
			roomID = Random.Range(0, 2000).ToString();

			foreach (RoomInfo i in _infos)
			{
				if (i.Id == roomID)
					same = true;
			}
		}

		client.Multiplayer.CreateJoinRoom
		(
			roomID,                                 //Room id. If set to null a random roomid is used
			"Battle",                               //The room type started on the server
			true,                                   //Should the room be visible in the lobby?
			null,
			null,
			(Connection _connection) => { OnJoinRoom(_connection); },
			(PlayerIOError _error) => { Fail(_error, "ListRoomsCreate"); }
		);
	}


	//Create
	public void CreateRoom()
	{
		client.Multiplayer.ListRooms
		(
			"Battle",
			null,
			10,
			0,
			(RoomInfo[] _infos) => { ListRoomsCreate(_infos); },
			(PlayerIOError _error) => { Fail(_error, "CreateRoom"); }
		);
	}

	private void JoinRoom()
	{
		client.Multiplayer.JoinRoom
		(
			roomID,
			null,
			(Connection _connection) => { OnJoinRoom(_connection); },
			(PlayerIOError _error) => { Fail(_error, "JoinRoom"); }
		);
	}



	private void OnJoinRoom(Connection _connection)
	{
		connection = _connection;
		connection.OnMessage += handlemessage;
	}

	private void Fail(PlayerIOError _error, string _name)
	{
		Debug.Log("Erreur" + _name);
	}


	//Loop
	void FixedUpdate()
	{
		foreach (Message m in messages)
		{
            #region Join/Left Room
            if (m.Type == "PlayerJoined")
			{
				if (!teamAttributed)
				{
					team = (e_teams)m.GetInt(0);
					teamAttributed = true;
				}
				if (m.GetInt(0) == 2)
				{
					if (currentStatus == e_status.Default)
					{
						GetComponent<SelectionInputs>().StartSelection(team);
						currentStatus = e_status.InSelection;
					}
				}
			}

			else if (m.Type == "PlayerLeft")
			{
				if (currentStatus == e_status.InSelection)
				{
					DisconnectMe();
				}
				else if (currentStatus == e_status.InGame)
				{
					gameplayManager.GameEnd();
					currentStatus = e_status.GameEnded;
				}
			}

			else if (m.Type == "FullRoom")
			{
				ListRoomsJoin(roomInfos.ToArray());
			}
            #endregion

            #region Initialization Checks
            else if (m.Type == "TeamValidated")
			{
				for (int i = 0; i < 6; i++)
				{
					heroes[i] = m.GetInt((uint)i);
				}

				SceneTransi.ToExecute exe = LoadGameplay;
				SceneTransi.Instance.Transi(true, exe);
			}

			else if (m.Type == "SceneLoaded")
			{
				gameplayManager = FindObjectOfType<GameplayManager>();

				gameplayManager.Init(this, team, heroes);
				gameplayManager.OnEnnemyTurn += EnnemyTurn;

				SceneTransi.Instance.Transi(false, null);

				Destroy(GetComponent<SelectionInputs>());
				currentStatus = e_status.InGame;
			}

			else if (m.Type == "PlacementValidated")
			{
				gameplayManager.BoardManager.PlacementValidated();
			}

			else if (m.Type == "GameStart")
			{
				gameplayManager.GameStart();
			}
			#endregion

			#region In Game
			else if (m.Type == "MoveHeroPiece")
			{
				int index = -1;
				index = m.GetInt(0);
				Vector2Int desti = new Vector2Int(m.GetInt(1), m.GetInt(2));
				bool useMove = m.GetBoolean(3);

				gameplayManager.BoardManager.MoveHeroPiece(index, desti, useMove);
			}

			else if (m.Type == "ModifyStatHeroPiece")
			{
				int index = -1;
				index = m.GetInt(0);
				e_stats key = (e_stats)m.GetInt(1);
				int value = m.GetInt(2);
				int duration = m.GetInt(3);
				int tick = m.GetInt(4);

				gameplayManager.BoardManager.ModifyStatHeroPiece(index, key, value, duration, tick);
			}

			else if (m.Type == "YourTurn")
			{
				gameplayManager.YourTurn();
			}

			else if (m.Type == "Eliminated")
			{
				gameplayManager.Eliminated((e_teams)m.GetInt(0));

				if (m.GetInt(1) <= 1)
				{
					gameplayManager.GameEnd();
					currentStatus = e_status.GameEnded;
				}
			}
            #endregion
        }

        messages.Clear();
	}

	private void handlemessage(object sender, Message m)
	{
		messages.Add(m);
	}


	//Actions
	public void ValidTeam(int[] _heroes)
	{
		object[] toSend = new object[] { (int)team - 1, _heroes[0], _heroes[1], _heroes[2] };

		if (connection != null)
			connection.Send("ValidTeam", toSend);
	}

	public void ValidPlacement()
	{
		if (connection != null)
			connection.Send("ValidPlacement");
	}

	public void GameStart()
	{
		if (connection != null)
			connection.Send("GameStart");
	}

	public void MoveHeroPiece(int _index, Vector2Int _desti, bool _useMove)
	{
		object[] toSend = new object[] { _index, _desti.x, _desti.y, _useMove };

		if (connection != null)
			connection.Send("MoveHeroPiece", toSend);
	}

	public void ModifyStatHeroPiece(int _index, e_stats _key, int _value, int _duration, int _tick)
	{
		object[] toSend = new object[] { _index, (int)_key, _value, _duration, _tick };

		if (connection != null)
			connection.Send("ModifyStatHeroPiece", toSend);
	}

	public void EnnemyTurn()
	{
		object[] toSend = new object[] { };

		if (connection != null)
			connection.Send("YourTurn", toSend);
	}

	public void Defeat()
	{
		if (connection != null)
			connection.Send("Eliminated", (int)team);
	}

	private void DisconnectMe()
	{
		if (connection != null)
			connection.Send("DisconnectMe");

		SceneManager.LoadScene(0);
		Destroy(gameObject);
	}

	public void Disconnect()
	{
		connection.Disconnect();
	}



	public void LoadGameplay()
	{
		StartCoroutine(LoadGameplayCorout());
	}

	private IEnumerator LoadGameplayCorout()
	{
		loadGameplay = SceneManager.LoadSceneAsync(1);

		while (!loadGameplay.isDone)
			yield return null;

		if (connection != null)
			connection.Send("SceneLoaded");
	}
}
