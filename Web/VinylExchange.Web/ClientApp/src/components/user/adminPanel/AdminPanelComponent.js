import React from "react";
import AddGenresContainer from "./addGenres/AddGenresContainer";
import RemoveGenresContainer from "./removeGenres/RemoveGenresContainer";

function AdminPanelComponent() {
  return (
    <div className="container-fluid">
      <div className="row border text-center">
        <div className="col-12">
          <h1 className="property-text">Administration Panel</h1>
        </div>
        <div className="col-6">
          <div className="row">
            <div className="col-12 border">
              <h1 className="property-text">Genres</h1>
            </div>
            <div className="col-6">
              <div className="row ">
                <div className="col-12 border">
                  <h3 className="property-text">Add Genres</h3>
                </div>
                <div className="col-12 border p-30">
                  <AddGenresContainer />
                </div>
              </div>
            </div>
            <div className="col-6">
              <div className="row">
                <div className="col-12 border">
                  <h3 className="property-text">Remove Genres</h3>
                  <div className="col-12 border p-30">
                    <RemoveGenresContainer />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default AdminPanelComponent;
