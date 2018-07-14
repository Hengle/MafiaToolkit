#include "3dsmaxsdk_preinclude.h"
#include "EDMExportClass.h"
#include "M2EDM.h"
#include "MeshNormalSpec.h"
#include "dummy.h"
#include "triobj.h"
#include "gutil.h"

#define EDM_EXPORT_CLASS_ID	Class_ID(0x14fe2fdc, 0x102f11a2)

class EDMExportClassDesc : public ClassDesc2
{
public:
	virtual int IsPublic() { return TRUE; }
	virtual void* Create(BOOL /*loading = FALSE*/) { return new EDMExport(); }
	virtual const TCHAR *	ClassName() { return GetString(IDS_EDM_EXPORT_CLASS); }
	virtual SClass_ID SuperClassID() { return SCENE_EXPORT_CLASS_ID; }
	virtual Class_ID ClassID() { return EDM_EXPORT_CLASS_ID; }
	virtual const TCHAR* Category() { return GetString(IDS_CATEGORY_EXPORT); }

	virtual const TCHAR* InternalName() { return _T("M2Plugin"); }	// returns fixed parsable name (scripter-visible name)
	virtual HINSTANCE HInstance() { return hInstance; }					// returns owning module handle


};
ClassDesc2* GetEDMExportDesc() {
	static EDMExportClassDesc EDMExportDesc;
	return &EDMExportDesc;
}

static FILE *stream = NULL;

//EDM EXPORT SECTION
//=========================

EDMExport::EDMExport() {
}
EDMExport::~EDMExport() {
}
int EDMExport::ExtCount() {
	return 1;
}
const TCHAR* EDMExport::Ext(int n) {
	switch (n) {
	case 0:
		return _T("EDM");
	}
	return _T("");
}
const TCHAR* EDMExport::LongDesc() {
	return GetString(IDS_EDM_L_DESC);
}
const TCHAR* EDMExport::ShortDesc() {
	return GetString(IDS_EDM_S_DESC);
}
const TCHAR* EDMExport::AuthorName() {
	return GetString(IDS_AUTHOR);
}
const TCHAR* EDMExport::CopyrightMessage() {
	return _T("");
}
const TCHAR* EDMExport::OtherMessage1() {
	return _T("");
}
const TCHAR* EDMExport::OtherMessage2() {
	return _T("");
}
unsigned int EDMExport::Version() {
	return 1;
}
void EDMExport::ShowAbout(HWND hWnd) {}
int EDMExport::DoExport(const MCHAR *name, ExpInterface *ei, Interface *i, BOOL suppressPrompts, DWORD options)
{
	//Check if nodes are selected.
	if (i->GetSelNodeCount() < 1)
	{
		MessageBox(NULL, _T("Select the root node of a mesh."), _T("Error!"), MB_OK);
		return FALSE;
	}

	//define file to write to.
	EDMExportWorkFile theFile(name, _T("wb"));
	FILE *stream = theFile.Stream();

	INode* parentNode = i->GetSelNode(0);

	MSTR mstr;
	parentNode->GetObjOrWSMRef()->GetClassName(mstr);

	//check if dummy.
	if (mstr != _T("Dummy"))
	{
		MessageBox(NULL, _T("Select a dummy node containing a mesh"), _T("Error!"), MB_OK);
		return FALSE;
	}

	//build file structure
	EDMStructure fileStructure = EDMStructure();
	fileStructure.SetName(parentNode->GetName());
	fileStructure.SetPartSize(parentNode->NumberOfChildren());

	std::vector<EDMPart> parts = std::vector<EDMPart>(fileStructure.GetPartSize());

	for (int i = 0; i != parts.size(); i++)
	{
		parts[i] = EDMPart();

		//Get child nodes (1 for now)
		INode* child = parentNode->GetChildNode(i);
		parts[i].SetName(child->GetName());

		//get TriObject for mesh
		TriObject* object = static_cast<TriObject*>(child->GetObjOrWSMRef());
		Mesh &mesh = object->mesh;
		
		//get verts and normals from mesh and save.
		parts[i].SetVertSize(mesh.numVerts);
		parts[i].SetIndicesSize(mesh.numFaces);
		parts[i].SetUVSize(mesh.numVerts);

		//init vectors
		std::vector<Point3> verts = std::vector<Point3>(mesh.numVerts);
		std::vector<Point3> normals = std::vector<Point3>(mesh.numVerts);
		std::vector<Point3> tangents = std::vector<Point3>(mesh.numVerts);
		std::vector<Point3> uvs = std::vector<Point3>(mesh.numVerts);
		std::vector<Int3> indices = std::vector<Int3>(mesh.numFaces);

		MeshNormalSpec* normalSpec = mesh.GetSpecifiedNormals();

		for (int c = 0; c != verts.size(); c++)
		{
			verts[c] = mesh.getVert(c);		
			normals[c] = normalSpec->Normal(c);
		}

		for (int c = 0; c != indices.size(); c++)
		{
			Int3 ind;
			ind.i1 = mesh.faces[c].v[0];
			ind.i2 = mesh.faces[c].v[1];
			ind.i3 = mesh.faces[c].v[2];

			indices[c] = ind;
		}

		MeshMap &map = mesh.Map(1);
		for (int c = 0; c != verts.size(); c++)
		{
			uvs[c].x = map.tv[c].x;
			uvs[c].y = map.tv[c].y;
			uvs[c].z = map.tv[c].z;
		}

		for (int c = 0; c != verts.size(); c++)
		{
			Point3 tangent;
			tangent = ComputeTangent(&map.tv[c], &verts[c]);
			tangents[c] = tangent;
		}

		parts[i].SetVertices(verts);
		parts[i].SetNormals(normals);
		parts[i].SetTangents(tangents);
		parts[i].SetIndices(indices);
		parts[i].SetUVs(uvs);
	}
	fileStructure.SetParts(parts);
	fileStructure.WriteToStream(stream);

	return TRUE;
}
