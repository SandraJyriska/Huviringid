<template>
  <div class="container">
    <div class="watch-notifications">
      <WatchNotifications />
    </div>
    <div class="upcoming-lessons">
      <UpcomingLessons />
    </div>
    <div class="small">
      <div class="student-extracurriculars">
        <StudentExtracurriculars />
      </div>
      <div class="add-note-button">
        <AddNoteButton />
      </div>
    </div>
    
  </div>


</template>

<style scoped>
.container {
  display: flex;
  align-self: center;
  /*position: relative;*/
  justify-content: space-between;
  max-width: 1400px;
}

.small {
  flex-direction: column;
  position:relative
}

.watch-notifications {
  position: left;  
  margin-right: 10px;
  margin-left: 15px;
  transition: all 0.3s ease;

}

.upcoming-lessons {
  position: center;  
  margin-left: 10px; /* Lisa vasakule marginaali, et see oleks paremal */
  transition: all 0.3s ease;

}

.add-note-button {
  margin-left: auto;
  margin-top: 100; 
  margin-bottom: 20px;
  transition: all 0.3s ease;

} 

.student-extracurriculars {
  position: right;
  margin-top: 20px; /* Väike kaugus ülevalt */
  flex-grow: 1;
  transition: all 0.3s ease;

}
</style>