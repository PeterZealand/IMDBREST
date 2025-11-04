Vue.createApp({
    data(){
        return{
            apiBase: "http://localhost:5041",
            actor:{
                primaryName:null,
                birthYear:null,
                deathYear:null,
                primaryProfessions:[],
                knownForTitles:[],
            },
            professions:[],
        }
    },
    async created(){
        this.getProfessions()
    },
    methods:{
        buildUrl(pathAndQuery) {
            const base = (this.apiBase || "").replace(/\/+$/, "");
            return base + pathAndQuery;
        },
        insertNewActor(){
            knownFor = this.actor.knownForTitles.split(',')

            newActor = {
                primaryName:this.actor.primaryName,
                birthYear:this.actor.birthYear,
                deathYear:this.actor.deathYear
            }
        },
        async getProfessions(){
            try{
                const url = this.buildUrl('/api/professions')
                const res = await axios.get(url)
                this.professions = res.data
            }
            catch(error){
                console.log(error)
            }
        },
        backToFront(){
            location.href = "./index.html"
        },
    }
}).mount("#newActor")
