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
            titleSearchTerm:null,
            titles:[],
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
        async insertNewActor(){
            const url = this.buildUrl('/api/actors')
            
            newActor = {
                primaryName:this.actor.primaryName,
                birthYear:this.actor.birthYear,
                deathYear:this.actor.deathYear,
                primaryProfessions:this.actor.primaryProfessions,
                knownForTitles: this.actor.knownForTitles
            }
            try{
                const res = await axios.post(url,newActor)
                window.alert("Actor inserted")
            }
            catch(error){
                window.alert(error)
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
        async searchTitle(){
            try{
                const url = this.buildUrl(`/api/Titles/Top?count=10&titleName=${this.titleSearchTerm}`)
                const res = await axios.get(url)
                this.titles = res.data
            }
            catch(error){
            }
        },
    }
}).mount("#newActor")
