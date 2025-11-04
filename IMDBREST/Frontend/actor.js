Vue.createApp({
    data(){
        return{
            actor:{
            },
        }
    },
    async created(){
        this.readQuery()
    },
    methods:{
        readQuery(){
            const urlParams = new URLSearchParams(window.location.search)
            const rawActor = urlParams.get('actor')
            this.baseURI = urlParams
            const decodedActor = rawActor ? decodeURIComponent(rawActor) : 'No Actor Found'
            this.actor = decodedActor
        },
        backToFront(){
            location.href = "./index.html"
        },
    }
}).mount("#actor")
