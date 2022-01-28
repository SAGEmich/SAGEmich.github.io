- 👋 Hi, I’m @SAGEmich
- 👀 I’m interested in Gameplay programing, algorithms and sports 
- 🌱 I’m currently working on Survival game
- 📫 How to reach me:

      Instagram: https://www.instagram.com/ej_lolo/

      Twitter: https://twitter.com/znajkiewicz

Here are some of my code and projects:

Custom gravity script

	static List<GravitySource> sources = new List<GravitySource>();
	
	public static Vector3 GetGravity(Vector3 position)
	{
		Vector3 g = Vector3.zero;

		for(int i = 0; i < sources.Count; i++)
		{
			g += sources[i].GetGravity(position);
		}

		return g;
	}
	
	public static Vector3 GetGravity(Vector3 position, out Vector3 upAxis)
	{
		Vector3 g = Vector3.zero;

		for(int i = 0; i < sources.Count; i++)
		{
			g += sources[i].GetGravity(position);
		}
		upAxis = -g.normalized;
		return g;
	}

	public static Vector3 GetUpAxis(Vector3 position)
	{
		Vector3 g = Vector3.zero;

		for(int i = 0; i < sources.Count; i++)
		{
			g += sources[i].GetGravity(position);
		}

		return -g.normalized;
	}

	public static void Register(GravitySource source)
	{
		Debug.Assert(!sources.Contains(source), "Duplicate registration of gravity source!", source);
		sources.Add(source);
	}

	public static void Unregister(GravitySource source)
	{
		Debug.Assert(!sources.Contains(source), "Unregistration of unknown gravity source!", source);
		sources.Remove(source);
	}

this script is used in my training project where I made smooth advanced movement

![Gravity ss](https://user-images.githubusercontent.com/58221747/151397500-ac6d97b9-ed98-42df-a5b6-85b6bd70bdd9.png)

Geometrics in Unity, it is project I had most fun working on

![torus](https://user-images.githubusercontent.com/58221747/151397817-b563d1fa-b682-41b5-923e-ee1de07ae68a.jpg)
![Sphere](https://user-images.githubusercontent.com/58221747/151397845-f116f99f-2b2f-456e-b096-1e39236029bc.png)

And some scripts for it

![Torus script](https://user-images.githubusercontent.com/58221747/151397982-2a0e6195-9454-4e9a-9fd3-ade8f2694192.png)
![Update funcion](https://user-images.githubusercontent.com/58221747/151398028-76631a63-68d2-4dc0-aa79-8416403d65a0.png)

I was working on fields cultivation in a bigger project called LinearVillager

![FieldsCult ss](https://user-images.githubusercontent.com/58221747/151426562-5208050b-aac9-4fc1-8fae-cba4a560fb4e.png)

And here is some of the code I wrote

        [SerializeField]
        private GameObject notPreparedPhase;
        [SerializeField]
        private GameObject readyToSeedPhase;
        [SerializeField]
        private GameObject[] grainPlantPhases;


        public override void UpdateVisualization(BaseLocationElementData databaseData)
        {
            base.UpdateVisualization(databaseData);

            notPreparedPhase.gameObject.SetActive(false);
            readyToSeedPhase.gameObject.SetActive(false);

            foreach(GameObject phase in grainPlantPhases)
            {
                phase.SetActive(false);
            }

            if(data.growthPhase == FieldGrowthPhase.NotPrepared)
            {
                notPreparedPhase.SetActive(true);
            }
            else if(data.growthPhase == FieldGrowthPhase.ReadyToSeed)
            {
                readyToSeedPhase.SetActive(true);
            }
            else
            {
                if(data.SeedItemConfigData.plantType == PlantType.Grain)
                {
                    grainPlantPhases[data.growthProgress].SetActive(true);
                }
            }

            //IS THIS REALLY OK??? - TO CHECK IT ONE DAY FOR POSSIBLE MEMORY LEAK???
            GameController.Instance.timeTickEvent -= OnTickEvent;
            GameController.Instance.timeTickEvent += OnTickEvent;
        }

        public override void InvokeInteraction(BaseCharacterController controller)
        {
            var characterController = controller as BaseHumanoidCharacterController;
            if(characterController == null)
            {
                return;
            }

            if(data.growthPhase == FieldGrowthPhase.NotPrepared && characterController.FirstHandWeaponConfig != null && characterController.FirstHandWeaponConfig.canPlowField)
            {
                characterController.Action_FieldPrepare();
                data.growthPhase = FieldGrowthPhase.ReadyToSeed;
            }

            if(data.growthPhase == FieldGrowthPhase.ReadyToSeed && characterController.FirstHandSeedConfig != null &&
               characterController.FirstHandSeedConfig.plantType != PlantType.None)
            {
                data.growthPhase = FieldGrowthPhase.Seeded;
                data.seedItemId = characterController.FirstHandSeedConfig.id;

                data.growthProgress = 0;
                data.phaseChangeTime = GameController.Instance.CurrentDate + BalanceConfig.Current.FieldPhaseGrowthTimeMinutes();

                characterController.Action_FieldSeeded(characterController.FirstHandSeedConfig.id);
            }

            if(data.growthPhase == FieldGrowthPhase.Harvest && characterController.FirstHandWeaponConfig != null && characterController.FirstHandWeaponConfig.canHarvestField)
            {
                characterController.Action_FieldHarvested(data.SeedItemConfigData);

                data.seedItemId = null;
                data.growthPhase = FieldGrowthPhase.NotPrepared;
            }

            UpdateVisualization(data);
        }


Crazy Metal Fighters - Tour based mobile fighter 
It was university project, we were working in 5 man team

![CMF ss](https://user-images.githubusercontent.com/58221747/151396626-77a02ecd-5e10-4de0-8a5b-172ba0cbf614.jpg)

Script used for rotating robot model in garage 

    [SerializeField] private float _rotSpeed = 10000f;
    private bool _dragging = false;
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMouseDrag()
    {
        _dragging = true;
    }

    void Update()
    {
      if(Input.GetMouseButtonUp(0))
        {
            _dragging = false;
        }
       
    }

    private void FixedUpdate()
    {
        if(_dragging)
        {
            float x = Input.GetAxis("Mouse X") * _rotSpeed * Time.fixedDeltaTime;
          
            rb.AddTorque(Vector3.down * x);
        }
    }

